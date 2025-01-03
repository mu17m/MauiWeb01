﻿using MauiBook.DataAccess.Data;
using MauiBook.Models;
using Microsoft.AspNetCore.Mvc;
using MauiBook.DataAccess.Repositry.IRepositry;
namespace MauiBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitofwork;
        public CategoryController(IUnitOfWork Db)
        {
            _unitofwork = Db;
        }
        public IActionResult Index()
        {
            List<Category> categories = _unitofwork.category.GetAll().ToList();
            return View(categories);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Category Model)
        {

            if (ModelState.IsValid)
            {
                _unitofwork.category.Add(Model);
                _unitofwork.Save();
                TempData["Sucess"] = "Category Created Sucessfully";
                return RedirectToAction("index");
            }
            return View(Model);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Category? category = _unitofwork.category.Get(C => C.Id == id);
            if (category == null)
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
                _unitofwork.category.Update(Model);
                _unitofwork.Save();
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
            Category? category = _unitofwork.category.Get(C => C.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            Category? Model = _unitofwork.category.Get(C => C.Id == id);
            if (Model == null)
            {
                return NotFound();
            }
            _unitofwork.category.Remove(Model);
            _unitofwork.Save();
            TempData["Sucess"] = "Category Deleted Sucessfully";
            return RedirectToAction("index");
        }

    }
}
