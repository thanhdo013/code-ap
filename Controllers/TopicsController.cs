using Microsoft.AspNetCore.Mvc;
using TrainingFPTCo.Models.Queries;
using TrainingFPTCo.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using TrainingFPTCo.Migrations;
using TrainingFPTCo.Helpers;
using TrainingFPT.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TrainingFPT.Controllers
{
    public class TopicsController : Controller
    {
        // INDEX
        [HttpGet]
        public IActionResult Index(string SearchString, string Status)
        {
            TopicsViewModel topic = new TopicsViewModel();
            topic.TopicDetailList = new List<TopicDetail>();
            var dataTopics = new TopicQuery().GetAllDataTopics(SearchString, Status);
            var courseQuery = new CourseQuery();
            foreach (var data in dataTopics)
            {
                string courseName = courseQuery.GetCourseNameById(data.CourseId);
                topic.TopicDetailList.Add(new TopicDetail
                {
                    Id = data.Id,
                    Name = data.Name,
                    CourseId = data.CourseId,
                    Description = data.Description,
                    ViewDocuments = data.ViewDocuments,
                    ViewAttachFile = data.ViewAttachFile,
                    ViewPosterTopic = data.ViewPosterTopic,
                    Status = data.Status,
                    viewCourseName = courseName
                });
            }
            ViewData["keyword"] = SearchString;
            ViewBag.Status = Status;
            return View(topic);
        }

        // ADD
        [HttpGet]
        public IActionResult Add()
        {
            TopicDetail topic = new TopicDetail();
            List<SelectListItem> items = new List<SelectListItem>();
            var dataCourses = new CourseQuery().GetAllDataCourses(null, null);
            foreach (var course in dataCourses)
            {
                items.Add(new SelectListItem
                {
                    Value = course.Id.ToString(),
                    Text = course.Name
                });
            }
            ViewBag.Courses = items;
            return View(topic);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(TopicDetail topic, IFormFile Documents, IFormFile AttachFile, IFormFile PosterTopic)
        {
            // return Ok(ModelState); // Loại bỏ kiểm tra ModelState.IsValid
            try
            {
                string documentsTopic = UploadFileHelper.UpLoadFile(Documents);
                string attachFileTopic = AttachFile != null ? UploadFileHelper.UpLoadFile(AttachFile) : null;
                string posterTopic = UploadFileHelper.UpLoadFile(PosterTopic);
                int idTopic = new TopicQuery().InsertDataTopic(
                    topic.Name,
                    topic.CourseId,
                    topic.Description,
                    documentsTopic,
                    attachFileTopic,
                    posterTopic,
                    topic.Status
                );
                if (idTopic > 0)
                {
                    TempData["saveStatus"] = true;
                }
                else
                {
                    TempData["saveStatus"] = false;
                }
                return RedirectToAction(nameof(TopicsController.Index), "Topics");
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        // DELETE
        [HttpGet]
        public IActionResult Delete(int id = 0)
        {
            bool del = new TopicQuery().DeleteItemTopic(id);
            if (del)
            {
                TempData["statusDel"] = true;
            }
            else
            {
                TempData["statusDel"] = false;
            }
            return RedirectToAction(nameof(TopicsController.Index), "Topics");
        }

        // UPDATE
        [HttpGet]
        public IActionResult Update(int id = 0)
        {
            TopicDetail topicDetail = new TopicQuery().GetDataTopicById(id);
            List<SelectListItem> items = new List<SelectListItem>();
            var dataCourses = new CourseQuery().GetAllDataCourses(null, null);
            foreach (var course in dataCourses)
            {
                items.Add(new SelectListItem
                {
                    Value = course.Id.ToString(),
                    Text = course.Name
                });
            }
            ViewBag.Courses = items;
            return View(topicDetail);
        }

        [HttpPost]
        public IActionResult Update(TopicDetail topicDetail, IFormFile Documents, IFormFile AttachFile, IFormFile PosterTopic)
        {
            try
            {
                var detail = new TopicQuery().GetDataTopicById(topicDetail.Id);
                string uniqueDocuments = detail.ViewDocuments;
                string uniqueAttachFile = detail.ViewAttachFile;
                string uniquePosterTopic = detail.ViewPosterTopic;

                if (Documents != null)
                {
                    uniqueDocuments = UploadFileHelper.UpLoadFile(Documents);
                }
                if (AttachFile != null)
                {
                    uniqueAttachFile = UploadFileHelper.UpLoadFile(AttachFile);
                }
                if (PosterTopic != null)
                {
                    uniquePosterTopic = UploadFileHelper.UpLoadFile(PosterTopic);
                }

                bool update = new TopicQuery().UpdateTopicById(
                    topicDetail.Name,
                    topicDetail.CourseId,
                    topicDetail.Description,
                    uniqueDocuments,
                    uniqueAttachFile,
                    uniquePosterTopic,
                    topicDetail.Status,
                    topicDetail.Id);

                if (update)
                {
                    TempData["updateStatus"] = true;
                }
                else
                {
                    TempData["updateStatus"] = false;
                }
                return RedirectToAction(nameof(TopicsController.Index), "Topics");
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }
    }
}
