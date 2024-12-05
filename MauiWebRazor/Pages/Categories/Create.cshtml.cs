using MauiWebRazor.Data;
using MauiWebRazor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MauiWebRazor.Pages.Categories
{
    [BindProperties]
    public class CreateModel : PageModel
    {
        public CreateModel(ApplicationDbContext Db)
        {
            _Db = Db;
        }

        private readonly ApplicationDbContext _Db;
        public Category category {  get; set; }

        public void OnGet()
        {
            
        }
        public IActionResult OnPost()
        {
            _Db.Add(category);
            _Db.SaveChanges();
            TempData["Sucess"] = "Category Created Sucessfully";
            return RedirectToPage("index");
        }
    }
}
