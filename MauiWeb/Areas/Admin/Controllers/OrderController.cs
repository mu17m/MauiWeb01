using MauiBook.DataAccess.Repositry;
using MauiBook.DataAccess.Repositry.IRepositry;
using MauiBook.Models;
using MauiBook.Models.ViewModels;
using MauiBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;

namespace MauiBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public OrderVM orderVM { get; set; }

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Details(int OrderId)
        {
            orderVM = new()
            {
                Order = _unitOfWork.orderHeaderRepositry.Get(u => u.Id == OrderId, includeProperity: "ApplicationUser"),
                OrderDetails = _unitOfWork.orderDetailRepositry.GetAll(u => u.OrderHeaderId == OrderId, includeProperity:"Product")
            };
            return View(orderVM);
        }
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult UpdateOrderDetails()
        {
            var OrderFromDB = _unitOfWork.orderHeaderRepositry.Get(u => u.Id == orderVM.Order.Id);
            OrderFromDB.Name = orderVM.Order.Name;
            OrderFromDB.PhoneNumber = orderVM.Order.PhoneNumber;
            OrderFromDB.StreetAddress = orderVM.Order.StreetAddress;
            OrderFromDB.City = orderVM.Order.City;
            OrderFromDB.State = orderVM.Order.State;
            OrderFromDB.PostalCode = orderVM.Order.PostalCode;
            if(orderVM.Order.Carrier != null)
            {
                OrderFromDB.Carrier = orderVM.Order.Carrier;
            }
            if(orderVM.Order.TrackingNumber != null)
            {
                OrderFromDB.TrackingNumber = orderVM.Order.TrackingNumber;
            }
            _unitOfWork.orderHeaderRepositry.Update(OrderFromDB);
            _unitOfWork.Save();
            TempData["Sucess"] = "Details updated successfully";
            return RedirectToAction(nameof(Details), new { OrderId = OrderFromDB.Id });
        }
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult StartProcessing()
        {
            _unitOfWork.orderHeaderRepositry.UpdateStatus(orderVM.Order.Id, SD.StatusInProcess);
            _unitOfWork.Save();
            TempData["Sucess"] = "Order Details Updated Successfully";
            return RedirectToAction(nameof(Details), new {OrderId = orderVM.Order.Id});
        }
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult ShipProcess()
        {
            OrderHeader OrderFromDb = _unitOfWork.orderHeaderRepositry.Get(u => u.Id == orderVM.Order.Id);
            OrderFromDb.TrackingNumber = orderVM.Order.TrackingNumber;
            OrderFromDb.Carrier = orderVM.Order.Carrier;
            OrderFromDb.OrderStatus = SD.StatusShipped;
            OrderFromDb.ShippingDate = DateTime.Now;
            if(OrderFromDb.PaymentStatus == SD.PaymentStatusDelayedPayment)
            {
                OrderFromDb.PaymentDueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(30));
            }
            _unitOfWork.orderHeaderRepositry.Update(OrderFromDb);
            _unitOfWork.Save();
            TempData["Sucess"] = "Order shipped Successfully";
            return RedirectToAction(nameof(Details), new { OrderId = orderVM.Order.Id });
        }
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult CancelOrder()
        {
            OrderHeader OrderFromDb = _unitOfWork.orderHeaderRepositry.Get(u => u.Id ==orderVM.Order.Id);
            if(OrderFromDb.PaymentStatus == SD.StatusApproved)
            {
                var option = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = OrderFromDb.PaymentInetetId
                };
                var service = new RefundService();
                _unitOfWork.orderHeaderRepositry.UpdateStatus(OrderFromDb.Id, SD.StatusCancelled, SD.StatusRefunded);

            }
            else
            {
                _unitOfWork.orderHeaderRepositry.UpdateStatus(OrderFromDb.Id, SD.StatusCancelled, SD.StatusCancelled);
            }
            _unitOfWork.Save();
            TempData["Sucess"] = "Order Cancelled Successfully";
            return RedirectToAction(nameof(Details), new { OrderId = orderVM.Order.Id });
        }
        [HttpPost]
        [ActionName("Details")]
        public IActionResult Details_Pay_Now()
        {
            orderVM.Order = _unitOfWork.orderHeaderRepositry.Get(u => u.Id == orderVM.Order.Id, includeProperity:"ApplicationUser");
            orderVM.OrderDetails = _unitOfWork.orderDetailRepositry.GetAll(u => u.OrderHeaderId == orderVM.Order.Id, includeProperity:"Product");

            // Stripe logic
            var domin = "https://localhost:7200/";
            var options = new SessionCreateOptions
            {
                SuccessUrl = domin + $"Admin/Order/PaymentConfirmation?OrderHeaderid={orderVM.Order.Id}",
                CancelUrl = domin + $"Admin/Order/details?OrderId={orderVM.Order.Id}",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
            };
            foreach (var cart in orderVM.OrderDetails)
            {
                var SessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)cart.Price * 100,
                        Currency = "usd",
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
            _unitOfWork.orderHeaderRepositry.UpdateStripePaymentId(orderVM.Order.Id, session.Id, session.PaymentIntentId);
            _unitOfWork.Save();
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }
        public IActionResult PaymentConfirmation(int OrderHeaderid)
        {
            OrderHeader orderHeader = _unitOfWork.orderHeaderRepositry.Get(u => u.Id == OrderHeaderid, includeProperity: "ApplicationUser");
            if (orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
            {
                // Company
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);
                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _unitOfWork.orderHeaderRepositry.UpdateStatus(OrderHeaderid, orderHeader.OrderStatus, SD.PaymentStatusApproved);
                    _unitOfWork.orderHeaderRepositry.UpdateStripePaymentId(OrderHeaderid, orderHeader.SessionId, session.PaymentIntentId);
                    _unitOfWork.Save();
                }
                
            }
            return View(OrderHeaderid);
        }



        [HttpGet]
        public IActionResult GetAll(string Status)
        {
            IEnumerable<OrderHeader> orders;

            if(User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee))
            {
                orders = _unitOfWork.orderHeaderRepositry.GetAll(includeProperity: "ApplicationUser").ToList();
            }
            else
            {
                var ClaimUser = (ClaimsIdentity)User.Identity;
                var UserId = ClaimUser.FindFirst(ClaimTypes.NameIdentifier).Value;

                orders = _unitOfWork.orderHeaderRepositry.GetAll(u => u.ApplicationUserId == UserId, includeProperity: "ApplicationUser");

            }
            switch (Status)
            {
                
                case "inprogress":
                    orders = orders.Where(u => u.OrderStatus== SD.StatusInProcess);
                    break;
                case "pending":
                    orders = orders.Where(u => u.OrderStatus == SD.StatusPending);
                    break;
                case "completed":
                    orders = orders.Where(u => u.OrderStatus== SD.StatusShipped);
                    break;
                case "approved":
                    orders = orders.Where(u => u.OrderStatus == SD.StatusApproved);
                    break;
                default:
                    break;

            }
            return Json(new { data = orders });
        }
    }
}
