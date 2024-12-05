using Maui.DataAccess.Data;
using Maui.Models;
using Microsoft.AspNetCore.Mvc;

namespace MauiWeb.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _Db;
        public CategoryController(ApplicationDbContext Db)
        {
            _Db = Db;
        }
        public IActionResult Index()
        {
            List<Category> categories = _Db.categories.ToList();
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
                _Db.categories.Add(Model);
                _Db.SaveChanges();
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
            Category? category = _Db.categories.Find(id);
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
                _Db.categories.Update(Model);
                _Db.SaveChanges();
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
            Category? category = _Db.categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            Category? Model = _Db.categories.Find(id);
            if(Model == null)
            {
                return NotFound();
            }
           _Db.categories.Remove(Model);
            _Db.SaveChanges();
            TempData["Sucess"] = "Category Deleted Sucessfully";
            return RedirectToAction("index");
        }

    }
}
