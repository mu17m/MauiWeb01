using MauiBook.DataAccess.Data;
using MauiBook.Models;
using MauiBook.Models.ViewModels;
using MauiBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MauiBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _Db;
        private readonly UserManager<IdentityUser> _UserManager;
        public UserController(ApplicationDbContext Db, UserManager<IdentityUser> userManager)
        {
            _Db = Db;
            _UserManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }
        
        public IActionResult RoleManagment(string userId)
        {
            var RoleId = _Db.UserRoles.FirstOrDefault(u => u.UserId == userId).RoleId;

            RoleManagmentVM RoleVM = new RoleManagmentVM()
            {
                User = _Db.ApplicationUsers.Include(u => u.company).FirstOrDefault(u => u.Id == userId),
                RoleList = _Db.Roles.Select(r => new SelectListItem
                {
                    Text = r.Name,
                    Value = r.Name
                }),
                CompanyList = _Db.companies.Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                })
            };
            if(RoleVM.User != null)
            {
                RoleVM.User.Role = _Db.Roles.FirstOrDefault(r => r.Id == RoleId).Name;
            }


            return View(RoleVM);
        }

        [HttpPost]
        public IActionResult RoleManagment(RoleManagmentVM RoleVM)
        {
            var RoleId = _Db.UserRoles.FirstOrDefault(r => r.UserId == RoleVM.User.Id).RoleId;
            var OldRole = _Db.Roles.FirstOrDefault(r => r.Id == RoleId).Name;
            ApplicationUser User = _Db.ApplicationUsers.FirstOrDefault(u => u.Id == RoleVM.User.Id);

            if(!(RoleVM.User.Role == OldRole))
            {
                //Role was updated
                if (RoleVM.User.Role == SD.Role_Company)
                {
                    User.CompanyId = RoleVM.User.CompanyId;
                }
                if (OldRole == SD.Role_Company)
                {
                    User.CompanyId = null;
                }
            }
            else
            {
                if (RoleVM.User.Role == SD.Role_Company)
                {
                    User.CompanyId = RoleVM.User.CompanyId;
                }
            }
                _Db.SaveChanges();
                _UserManager.RemoveFromRoleAsync(User, OldRole).GetAwaiter().GetResult();
                _UserManager.AddToRoleAsync(User, RoleVM.User.Role).GetAwaiter().GetResult();
        return RedirectToAction("Index");
        }



        #region API calls
        [HttpGet]
        public IActionResult GetAll()
        {
            List<ApplicationUser> ListOfUsers = _Db.ApplicationUsers.Include(u => u.company).ToList();

            var UserRoles = _Db.UserRoles.ToList();
            var Roles = _Db.Roles.ToList();

            foreach (var user in ListOfUsers)
            {
                var RoleId = UserRoles.FirstOrDefault(u => u.UserId == user.Id).RoleId;
                user.Role = Roles.FirstOrDefault(u => u.Id == RoleId).Name;
                if (user.company == null)
                {
                    user.company = new() { Name = "" };
                }

            }

            return Json(new { data = ListOfUsers });
        }

        [HttpPost]
        public IActionResult LockUnlock([FromBody] string id)
        {
            var user = _Db.ApplicationUsers.FirstOrDefault(u => u.Id == id);

            if (user == null)
            {
                return Json(new { success = true, message = "Deleted Successfully" });
            }

            if(user.LockoutEnd != null && user.LockoutEnd > DateTime.Now)
            {
                //user is locked
                user.LockoutEnd = DateTime.Now;
            }
            else
            {
                //user is NOT locked
                user.LockoutEnd = DateTime.Now.AddYears(1000);
            }
            _Db.SaveChanges();
            return Json(new { success = true});
        }

        #endregion
    }
}
