using MauiWebRazor.Data;
using MauiWebRazor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MauiWebRazor.Pages.Categories
{
    [BindProperties]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _Db;
        public Category category { get; set; }

        public EditModel(ApplicationDbContext Db)
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
            if(ModelState.IsValid)
            {
                _Db.categories.Update(category);
                _Db.SaveChanges();
                TempData["Sucess"] = "Category Edited Sucessfully";
                return RedirectToPage("index");
            }
            return Page();
        }

    }
}
