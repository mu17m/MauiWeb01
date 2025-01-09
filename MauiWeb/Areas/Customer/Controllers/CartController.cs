using MauiBook.DataAccess.Repositry.IRepositry;
using MauiBook.Models;
using MauiBook.Models.ViewModels;
using MauiBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;

namespace MauiBookWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        public IUnitOfWork _UnitOfWork { get; set; }
        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }
        public CartController(IUnitOfWork unitOfWork)
        {
            _UnitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var CalimId = (ClaimsIdentity)User.Identity;
            var UserId = CalimId.FindFirst(ClaimTypes.NameIdentifier).Value;
            ShoppingCartVM = new()
            {
                ShoppingCartList = _UnitOfWork.ShoppingCartRepositry.GetAll(u => u.ApplicationUserId == UserId, includeProperity: "Product"),
                OrderHeader = new OrderHeader()

            };
            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetOrderTotal(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }
            return View(ShoppingCartVM);
        }
        public IActionResult Summary()
        {
            var CalimId = (ClaimsIdentity)User.Identity;
            var UserId = CalimId.FindFirst(ClaimTypes.NameIdentifier).Value;
            ShoppingCartVM = new()
            {
                ShoppingCartList = _UnitOfWork.ShoppingCartRepositry.GetAll(u => u.ApplicationUserId == UserId, includeProperity: "Product"),
                OrderHeader = new OrderHeader()

            };
            ShoppingCartVM.OrderHeader.ApplicationUser = _UnitOfWork.applicationUserRepositry.Get(u => u.Id == UserId);

            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
            ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
            ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
            ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
            ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;

            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetOrderTotal(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }
            return View(ShoppingCartVM);
        }
        [HttpPost]
        [ActionName("Summary")]
        public IActionResult SummaryPost()
        {
            var CalimId = (ClaimsIdentity)User.Identity;
            var UserId = CalimId.FindFirst(ClaimTypes.NameIdentifier).Value;
            ShoppingCartVM.ShoppingCartList = _UnitOfWork.ShoppingCartRepositry.GetAll(u => u.ApplicationUserId == UserId, includeProperity: "Product");

            ShoppingCartVM.OrderHeader.OrderDate = System.DateTime.Now;
            ShoppingCartVM.OrderHeader.ApplicationUserId = UserId;
            ApplicationUser applicationUser = _UnitOfWork.applicationUserRepositry.Get(u => u.Id == UserId);
            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetOrderTotal(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            if (applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                // this is Customer & we need to get payment
                ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
                ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
            }
            else
            {
                // this is company
                ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
                ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusApproved;
            }
            _UnitOfWork.orderHeaderRepositry.Add(ShoppingCartVM.OrderHeader);
            _UnitOfWork.Save();

            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                OrderDetail orderDetail = new()
                {
                    ProductId = cart.ProductId,
                    OrderHeaderId = ShoppingCartVM.OrderHeader.Id,
                    Price = cart.Price,
                    Count = cart.Count
                };
                _UnitOfWork.orderDetailRepositry.Add(orderDetail);
                _UnitOfWork.Save();
            }
            if (applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                // this is Customer & we need to get payment
                // Stripe logic
                var domin = "https://localhost:7200/";
                var options = new Stripe.Checkout.SessionCreateOptions
                {
                    SuccessUrl = domin + $"Customer/Cart/OrderConfirmation?id={ShoppingCartVM.OrderHeader.Id}",
                    CancelUrl = domin + "Customer/Cart/Index",
                    LineItems = new List<Stripe.Checkout.SessionLineItemOptions>(),
                    Mode = "payment",
                };
                foreach(var cart in ShoppingCartVM.ShoppingCartList)
                {
                    var SessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)cart.Price * 100,
                            Currency="usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = cart.Product.Title
                            }
                        },
                        Quantity = cart.Count,
                    };
                    options.LineItems.Add(SessionLineItem);
                }
                var service = new Stripe.Checkout.SessionService();
                Session session = service.Create(options);
                _UnitOfWork.orderHeaderRepositry.UpdateStripePaymentId(ShoppingCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
                _UnitOfWork.Save();
                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);

            }

            return RedirectToAction(nameof(OrderConfirmation), new { id = ShoppingCartVM.OrderHeader.Id });
        }
        public IActionResult OrderConfirmation(int id)
        {
            OrderHeader orderHeader = _UnitOfWork.orderHeaderRepositry.Get(u => u.Id == id, includeProperity:"ApplicationUser");
            if(orderHeader.PaymentStatus != SD.PaymentStatusDelayedPayment)
            {
                // Customer
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);
                if(session.PaymentStatus.ToLower() == "paid")
                {
                    _UnitOfWork.orderHeaderRepositry.UpdateStatus(id, SD.StatusApproved, SD.PaymentStatusApproved);
                    _UnitOfWork.orderHeaderRepositry.UpdateStripePaymentId(id, orderHeader.SessionId, session.PaymentIntentId);
                    _UnitOfWork.Save();
                }
                HttpContext.Session.Clear();

                List<ShoppingCart> shoppingCarts = _UnitOfWork.ShoppingCartRepositry.GetAll(u => u.ApplicationUserId == orderHeader.ApplicationUserId).ToList();
                _UnitOfWork.ShoppingCartRepositry.RemoveRange(shoppingCarts);
                _UnitOfWork.Save();
            }
            return View(id);
        }
        public IActionResult Plus(int cartId)
        {
            ShoppingCart CartFromDb = _UnitOfWork.ShoppingCartRepositry.Get(u => u.Id == cartId);
            CartFromDb.Count++;
            _UnitOfWork.ShoppingCartRepositry.Update(CartFromDb);
            _UnitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Minus(int cartId)
        {
            ShoppingCart CartFromDb = _UnitOfWork.ShoppingCartRepositry.Get(u => u.Id == cartId, tracked:true);
            if (CartFromDb.Count <= 1)
            {
                HttpContext.Session.SetInt32(SD.SessionCart, _UnitOfWork.ShoppingCartRepositry.GetAll(u => u.ApplicationUserId == CartFromDb.ApplicationUserId).Count() - 1);
                _UnitOfWork.ShoppingCartRepositry.Remove(CartFromDb);
                _UnitOfWork.Save();
            }
            else
            {
                CartFromDb.Count--;
                _UnitOfWork.ShoppingCartRepositry.Update(CartFromDb);
                _UnitOfWork.Save();
            }
            return RedirectToAction(nameof(Index));
        }
        public IActionResult remove(int cartId)
        {
            ShoppingCart CartFromDb = _UnitOfWork.ShoppingCartRepositry.Get(u => u.Id == cartId, tracked:true);
            HttpContext.Session.SetInt32(SD.SessionCart, _UnitOfWork.ShoppingCartRepositry.GetAll(u => u.ApplicationUserId == CartFromDb.ApplicationUserId).Count() - 1);

            _UnitOfWork.ShoppingCartRepositry.Remove(CartFromDb);
            _UnitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        private double GetOrderTotal(ShoppingCart shoppingCart)
        {
            if (shoppingCart.Product != null)
            {
                if (shoppingCart.Count <= 50)
                {
                    return shoppingCart.Product.Price;
                }
                else if (shoppingCart.Count <= 100)
                {
                    return shoppingCart.Product.Price50;
                }
                else return shoppingCart.Product.Price100;
            }
            return -1;
        }
    }
}
