using MauiBook.DataAccess.Data;
using MauiBook.Models;
using MauiBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MauiBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _Db;

        public UserController(ApplicationDbContext Db)
        {
            _Db = Db;
        }
        public IActionResult Index()
        {
            return View();
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

        [HttpDelete]
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
            return Json(new { success = true, message = "Deleted Successfully" });
        }

        #endregion
    }
}
