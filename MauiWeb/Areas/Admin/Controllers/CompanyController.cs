using MauiBook.DataAccess.Repositry;
using MauiBook.DataAccess.Repositry.IRepositry;
using MauiBook.Models.ViewModels;
using MauiBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Cryptography.X509Certificates;
using MauiBook.Models;

namespace MauiBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _UnitOfWork;

        public CompanyController(IUnitOfWork unitOfWork)
        {
            _UnitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            List<Company> Companys = _UnitOfWork.companayRepositry.GetAll().ToList();
            return View(Companys);
        }

        public IActionResult Upsert(int? id)
        {
            if (id == null || id == 0)
            {
                //Create
                Company company = new Company();
                return View(company);

            }
            else
            {
                //Update
                Company company = _UnitOfWork.companayRepositry.Get(c => c.Id == id);
                return View (company);
            }
        }
        [HttpPost]
        public IActionResult Upsert(Company entity)
        {
            if(ModelState.IsValid)
            {
                if(entity.Id == 0)
                {
                    _UnitOfWork.companayRepositry.Add(entity);
                    TempData["Sucess"] = "Company Created Sucessfully";
                }
                else
                {
                    _UnitOfWork.companayRepositry.Update(entity);
                    TempData["Sucess"] = "Company Edited Sucessfully";
                }
                _UnitOfWork.Save();
                return RedirectToAction("index");
            }
            return View(entity);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            Company? C1 = _UnitOfWork.companayRepositry.Get(P => P.Id == id);
            if (C1 == null)
            {
                return NotFound();
            }
            _UnitOfWork.companayRepositry.Remove(C1);
            _UnitOfWork.Save();
            TempData["Sucess"] = "Company Deleted Sucessfully";
            return RedirectToAction("index");
        }

        #region API CALL
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Company> Companys = _UnitOfWork.companayRepositry.GetAll().ToList();
            return Json(new { data = Companys });
        }

        public IActionResult Delete(int? id)
        {
            Company company = _UnitOfWork.companayRepositry.Get(p => p.Id == id);
            if(company == null)
            {
                return Json(new { success = false, message = "error while deleting" });
            }
            _UnitOfWork.companayRepositry.Remove(company);
            _UnitOfWork.Save();
            return Json(new { success = true, message = "deleted successfully!" });
        }
        #endregion
    }
}
