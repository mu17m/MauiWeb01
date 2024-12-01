using MauiWeb.Data;
using MauiWeb.Models;
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

    }
}
