using MauiBook.DataAccess.Repositry.IRepositry;
using MauiBook.Models;
using MauiBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace MauiBookWeb.Controllers
{
[Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var ClaimIdentity = (ClaimsIdentity)User.Identity;
            var ClaimedUser = ClaimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if(ClaimedUser != null)
            {
                HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCartRepositry.GetAll(c => c.ApplicationUserId == ClaimedUser.Value).Count());
            }
            IEnumerable<Product> products = _unitOfWork.productRepositry.GetAll(includeProperity: "category");
            return View(products);
        }

        public IActionResult Details(int id)
        {
            ShoppingCart Cart = new()
            {
                Product = _unitOfWork.productRepositry.Get(p => p.Id == id, includeProperity: "category"),
                Count = 1,
                ProductId = id
            }; 
            return View(Cart);
        }
        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            var ClaimId = (ClaimsIdentity)User.Identity;
            var UserId = ClaimId.FindFirst(ClaimTypes.NameIdentifier).Value;
            shoppingCart.ApplicationUserId = UserId;
            //shoppingCart.Id = 0;

            ShoppingCart cartFromDb = _unitOfWork.ShoppingCartRepositry.Get(c => c.ApplicationUserId == UserId && c.ProductId == shoppingCart.ProductId);
            if(cartFromDb != null)
            {
                //Update Exist Cart
                cartFromDb.Count += shoppingCart.Count;
                _unitOfWork.ShoppingCartRepositry.Update(cartFromDb);
                _unitOfWork.Save();
            }
            else
            {
                //Add New Cart
                _unitOfWork.ShoppingCartRepositry.Add(shoppingCart);
                HttpContext.Session.SetInt32(SD.SessionCart, 
                _unitOfWork.ShoppingCartRepositry.GetAll(c => c.ApplicationUserId == UserId).Count());
                _unitOfWork.Save();
            }
            
            TempData["Sucess"] = "Cart Updated Successfully";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
