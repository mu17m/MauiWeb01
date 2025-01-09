using MauiBook.DataAccess.Repositry;
using MauiBook.DataAccess.Repositry.IRepositry;
using MauiBook.Models;
using MauiBook.Models.ViewModels;
using MauiBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Cryptography.X509Certificates;

namespace MauiBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _UnitOfWork;
        private IWebHostEnvironment _webHost;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHost)
        {
            _UnitOfWork = unitOfWork;
            _webHost = webHost;
        }

        public IActionResult Index()
        {
            List<Product> Products = _UnitOfWork.productRepositry.GetAll(includeProperity:"category").ToList();
            return View(Products);
        }

        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new ProductVM
            {
                CategoryList = _UnitOfWork.categoryRepositry
                .GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                })
            };
            if (id == null || id == 0)
            {
                //Create
                productVM.product = new Product();
                return View(productVM);

            }
            else
            {
                //Update
                productVM.product = _UnitOfWork.productRepositry.Get(c => c.Id == id);
                return View (productVM);
            }
        }
        [HttpPost]
        public IActionResult Upsert(ProductVM entity, IFormFile? file)
        {
            if(ModelState.IsValid)
            {
                string WWWRoot = _webHost.WebRootPath;
                if(file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string filePath = Path.Combine(WWWRoot, @"Images\Product");

                    if(!string.IsNullOrEmpty(entity.product.ImageUrl))
                    {
                        var oldPath = Path.Combine(WWWRoot, entity.product.ImageUrl.TrimStart('\\'));

                        if(System.IO.File.Exists(oldPath))
                        {
                            System.IO.File.Delete(oldPath);
                        }
                    }
                    using (var fileStream = new FileStream(Path.Combine(filePath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    entity.product.ImageUrl = @"\Images\Product\" + fileName;
                }
                if(entity.product.Id == 0)
                {
                    _UnitOfWork.productRepositry.Add(entity.product);
                    TempData["Sucess"] = "Product Created Sucessfully";
                }
                else
                {
                    _UnitOfWork.productRepositry.Update(entity.product);
                    TempData["Sucess"] = "Product Edited Sucessfully";
                }
                _UnitOfWork.Save();
                return RedirectToAction("index");
            }
            return View(entity);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            Product? P1 = _UnitOfWork.productRepositry.Get(P => P.Id == id);
            if (P1 == null)
            {
                return NotFound();
            }
            _UnitOfWork.productRepositry.Remove(P1);
            _UnitOfWork.Save();
            TempData["Sucess"] = "Product Deleted Sucessfully";
            return RedirectToAction("index");
        }

        #region API CALL
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> Products = _UnitOfWork.productRepositry.GetAll(includeProperity: "category").ToList();
            return Json(new { data = Products });
        }

        public IActionResult Delete(int? id)
        {
            Product entity = _UnitOfWork.productRepositry.Get(p => p.Id == id);
            if(entity == null)
            {
                return Json(new { success = false, message = "error while deleting" });
            }
            var oldPath = Path.Combine(_webHost.ContentRootPath, entity.ImageUrl.TrimStart('\\'));
            if(System.IO.File.Exists(oldPath))
            {
                System.IO.File.Delete(oldPath);
            }
            _UnitOfWork.productRepositry.Remove(entity);
            _UnitOfWork.Save();
            return Json(new { success = true, message = "deleted successfully!" });
        }
        #endregion
    }
}
