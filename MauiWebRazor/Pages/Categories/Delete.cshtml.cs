using MauiWebRazor.Data;
using MauiWebRazor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MauiWebRazor.Pages.Categories
{
    [BindProperties]
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _Db;
        public Category category { get; set; }

        public DeleteModel(ApplicationDbContext Db)
        {
            _Db = Db;
        }

        public void OnGet(int? id)
        {
            if (id != null && id != 0)
            {
                category = _Db.categories.Find(id);
            }
        }
        public IActionResult OnPost()
        {
            Category? Model = _Db.categories.Find(category.Id);
            if (Model == null)
            {
                return NotFound();
            }
            _Db.categories.Remove(Model);
            _Db.SaveChanges();
            TempData["Sucess"] = "Category Deleted Sucessfully";
            return RedirectToPage("index");
        }
    }
}
