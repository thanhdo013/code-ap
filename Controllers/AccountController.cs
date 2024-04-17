using Microsoft.AspNetCore.Mvc;
using TrainingFPTCo.DataDBContext;
using TrainingFPTCo.Models;
using TrainingFPTCo.Models.Queries;
using TrainingFPTCo.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TrainingFPTCo.Controllers
{
    public class AccountController : Controller
    {
        private readonly TrainingDbContext _dbContext;

        public AccountController(TrainingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Index(string searchString, string filterStatus)
        {
            AccountViewModel accountModel = new AccountViewModel();
            accountModel.AccountDetailList = new List<AccountDetail>();
            var dataAccounts = new AccountQuery().GetAllAccounts(searchString, filterStatus);
            foreach (var item in dataAccounts)
            {
                var accountDetail = item;
                accountDetail.Role = new AccountQuery().GetRoleById(accountDetail.RoleId);
                accountModel.AccountDetailList.Add(new AccountDetail
                {
                    Id = item.Id,
                    RoleId = item.RoleId,
                    Role = item.Role,
                    Username = item.Username,
                    ExtraCode = item.ExtraCode,
                    Password = item.Password,
                    Email = item.Email,
                    Phone = item.Phone,
                    Address = item.Address,
                    FullName = item.FullName,
                    FirstName = item.FirstName,
                    LastName = item.LastName,
                    Birthday = item.Birthday,
                    Gender = item.Gender,
                    Education = item.Education,
                    ProgramingLanguage = item.ProgramingLanguage,
                    ToeicScore = item.ToeicScore,
                    Skills = item.Skills,
                    IPCleant = item.IPCleant,
                    LastLogin = item.LastLogin,
                    LastLogout = item.LastLogout,
                    CreatedAt = item.CreatedAt,
                    UpdatedAt = item.UpdatedAt,
                    DeletedAt = item.DeletedAt
                });
            }

            ViewData["currentFilter"] = searchString;
            ViewBag.FilterStatus = filterStatus;
            return View(accountModel);
        }


        [HttpGet]
        public IActionResult Add()
        {
            AccountAddViewModel account = new AccountAddViewModel();
            List<SelectListItem> roles = new List<SelectListItem>();
            var dataRoles = new RoleQuery().GetAllRoles(); // Thay RoleQuery bằng lớp chứa phương thức truy vấn tất cả các role từ cơ sở dữ liệu
            foreach (var role in dataRoles)
            {
                roles.Add(new SelectListItem
                {
                    Value = role.Id.ToString(),
                    Text = role.Name
                });
            }
            ViewBag.Roles = roles;
            return View(account);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AccountAddViewModel account)
        {
            //return Ok(ModelState);
            if (ModelState.IsValid)
            {
                try
                {
                    int idAccount = new AccountQuery().InsertAccount(
                        account.RoleId,
                        account.Username,
                        account.Password,
                        account.ExtraCode,
                        account.Email,
                        account.Phone,
                        account.Address,
                        account.FullName,
                        account.FirstName,
                        account.LastName,
                        account.Birthday,
                        account.Gender,
                        account.Education,
                        account.ProgramingLanguage,
                        account.ToeicScore,
                        account.Skills,
                        account.IPCleant
                    );

                    if (idAccount > 0)
                    {
                        TempData["saveStatus"] = true;
                    }
                    else
                    {
                        TempData["saveStatus"] = false;
                    }
                    return RedirectToAction(nameof(AccountController.Index), "Account");
                }
                catch (Exception ex)
                {
                    return Ok(ex.Message);
                }
            }
            List<SelectListItem> roles = new List<SelectListItem>();
            var dataRoles = new RoleQuery().GetAllRoles(); // Thay RoleQuery bằng lớp chứa phương thức truy vấn tất cả các role từ cơ sở dữ liệu
            foreach (var role in dataRoles)
            {
                roles.Add(new SelectListItem
                {
                    Value = role.Id.ToString(),
                    Text = role.Name
                });
            }
            ViewBag.Roles = roles;
            return View(account);
        }



        [HttpGet]
        public IActionResult Update(int id = 0)
        {
            AccountDetail accountDetail = new AccountQuery().GetAccountById(id);
            List<SelectListItem> items = new List<SelectListItem>();
            var dataRoles = new RoleQuery().GetAllRoles(); // Thay thế GetAllRoles bằng phương thức lấy danh sách các vai trò từ cơ sở dữ liệu của bạn
            foreach (var role in dataRoles)
            {
                items.Add(new SelectListItem
                {
                    Value = role.Id.ToString(),
                    Text = role.Name
                });
            }
            ViewBag.Roles = items;
            return View(accountDetail);
        }



        [HttpPost]
        public IActionResult Update(AccountDetail accountDetail)
        {
            try
            {
                bool updateAccount = new AccountQuery().UpdateAccountById(
                    accountDetail.RoleId,
                    accountDetail.Username,
                    accountDetail.Password,
                    accountDetail.ExtraCode,
                    accountDetail.Email,
                    accountDetail.Phone,
                    accountDetail.Address,
                    accountDetail.FullName,
                    accountDetail.FirstName,
                    accountDetail.LastName,
                    accountDetail.Birthday,
                    accountDetail.Gender,
                    accountDetail.Education,
                    accountDetail.ProgramingLanguage,
                    accountDetail.ToeicScore,
                    accountDetail.Skills,
                    accountDetail.IPCleant,
                    accountDetail.Id
                );

                if (updateAccount)
                {
                    TempData["updateStatus"] = true;
                }
                else
                {
                    TempData["updateStatus"] = false;
                }

                return RedirectToAction(nameof(AccountController.Index), "Account");
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }


        [HttpGet]
        public IActionResult Delete(int id = 0)
        {
            bool deleteAccount = new AccountQuery().DeleteAccountById(id);
            if (deleteAccount)
            {
                TempData["statusDelete"] = true; // Deleted successfully
            }
            else
            {
                TempData["statusDelete"] = false; // Delete failed
            }
            return RedirectToAction(nameof(AccountController.Index), "Account");
        }
    }
}
