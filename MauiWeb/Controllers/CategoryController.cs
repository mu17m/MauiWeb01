using MauiBook.DataAccess.Data;
using MauiBook.Models;
using Microsoft.AspNetCore.Mvc;
using MauiBook.DataAccess.Repositry.IRepositry;
namespace MauiBookWeb.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryRepositry _CategoryRepo;
        public CategoryController(ICategoryRepositry Db)
        {
            _CategoryRepo = Db;
        }
        public IActionResult Index()
        {
            List<Category> categories = _CategoryRepo.GetAll().ToList();
            return View(categories);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Category Model)
        {
          
           if(ModelState.IsValid)
            {
                _CategoryRepo.Add(Model);
                _CategoryRepo.Save();
                TempData["Sucess"] = "Category Created Sucessfully";
                return RedirectToAction("index");
            }
           return View(Model);
        }

        public IActionResult Edit(int? id)
        {
            if(id == null || id == 0 )
            {
                return NotFound();
            }
            Category? category = _CategoryRepo.Get(C => C.Id == id);
            if(category == null)
            {
                return NotFound();
            }
            return View(category);
        }
        [HttpPost]
        public IActionResult Edit(Category Model)
        {

            if (ModelState.IsValid)
            {
                _CategoryRepo.Update(Model);
                _CategoryRepo.Save();
                TempData["Sucess"] = "Category Edited Sucessfully";
                return RedirectToAction("index");
            }
            return View(Model);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Category? category = _CategoryRepo.Get(C => C.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            Category? Model = _CategoryRepo.Get(C => C.Id == id);
            if(Model == null)
            {
                return NotFound();
            }
           _CategoryRepo.Remove(Model);
            _CategoryRepo.Save();
            TempData["Sucess"] = "Category Deleted Sucessfully";
            return RedirectToAction("index");
        }

    }
}
