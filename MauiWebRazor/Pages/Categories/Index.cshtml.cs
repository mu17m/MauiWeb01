using MauiWebRazor.Data;
using MauiWebRazor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MauiWebRazor.Pages.Categories
{
    public class IndexModel : PageModel
    {
        public IndexModel(ApplicationDbContext Db)
        {
            _Db = Db;
        }

        private readonly ApplicationDbContext _Db;
        public List<Category> Categories { get; set; }

        public void OnGet()
        {
            Categories = _Db.categories.ToList();
        }
    }
}
