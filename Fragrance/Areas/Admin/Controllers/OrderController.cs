using Fragrance.DataAccess.Repository.IRepository;
using Fragrance.Models;
using Fragrance.Models.ViewModels;
using Fragrance.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using Stripe;
using System.Security.Claims;
using System.Diagnostics;

namespace Fragrance.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        public OrderVM OrderVM { get; set; }

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int orderId)
        {
            OrderVM = new()
            {
                OrderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == orderId, includeProperties: "ApplicationUser"),
                OrderDetail = _unitOfWork.OrderDetail.GetAll(u => u.OrderHeaderId == orderId, includeProperties: "Parfume")
            };
           
            return View(OrderVM);
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult UpdateOrderDetail()
        {
            var orderFromDb = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVM.OrderHeader.Id);
            if (orderFromDb == null)
            {
                TempData["error"] = "Order not found.";
                return RedirectToAction(nameof(Index));
            }
           
            orderFromDb.Name = OrderVM.OrderHeader.Name;
            orderFromDb.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;
            orderFromDb.StreetAddress = OrderVM.OrderHeader.StreetAddress;
            orderFromDb.City = OrderVM.OrderHeader.City;
            orderFromDb.State = OrderVM.OrderHeader.State;
            orderFromDb.PostalCode = OrderVM.OrderHeader.PostalCode;
            if (!string.IsNullOrEmpty(OrderVM.OrderHeader.Carrier))
            {
                orderFromDb.Carrier = OrderVM.OrderHeader.Carrier;
            }
            if (!string.IsNullOrEmpty(OrderVM.OrderHeader.TrackingNumber))
            {
                orderFromDb.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
            }
            orderFromDb.PaymentStatus = orderFromDb.PaymentStatus ?? OrderVM.OrderHeader.PaymentStatus;
            _unitOfWork.OrderHeader.Update(orderFromDb);
            _unitOfWork.Save();
            TempData["success"] = "Order Details Updated Successfully";
            return RedirectToAction(nameof(Details), new { orderId = orderFromDb.Id });
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult StartProcessing()
        {
            var orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVM.OrderHeader.Id);
            if (orderHeader == null)
            {
                TempData["error"] = "Order not found.";
                return RedirectToAction(nameof(Index));
            }
            
            _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, SD.StatusInProcess, orderHeader.PaymentStatus);
            _unitOfWork.Save();
           
            TempData["success"] = "Order Details Updated Successfully";
            return RedirectToAction(nameof(Details), new { orderId = orderHeader.Id });
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult ShipOrder()
        {
            var orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVM.OrderHeader.Id);
            if (orderHeader == null)
            {
                TempData["error"] = "Order not found.";
                return RedirectToAction(nameof(Index));
            }
            
            orderHeader.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
            orderHeader.Carrier = OrderVM.OrderHeader.Carrier;
            orderHeader.OrderStatus = SD.StatusShipped;
            orderHeader.ShippingDate = DateTime.Now;

           
            _unitOfWork.OrderHeader.Update(orderHeader);
            _unitOfWork.Save();
           
            TempData["success"] = "Order Shipped Successfully";
            return RedirectToAction(nameof(Details), new { orderId = orderHeader.Id });
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult CancelOrder()
        {
            var orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVM.OrderHeader.Id);
            if (orderHeader == null)
            {
                TempData["error"] = "Order not found.";
                return RedirectToAction(nameof(Index));
            }                                             
            _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusCancelled);           
            _unitOfWork.Save();
            TempData["success"] = "Order Cancelled Successfully.";
            return RedirectToAction(nameof(Details), new { orderId = orderHeader.Id });
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll(string status)
        {
            IEnumerable<OrderHeader> objOrderHeaders;
            if (User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee))
            {
                objOrderHeaders = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser").ToList();
            }
            else
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
                objOrderHeaders = _unitOfWork.OrderHeader
                    .GetAll(u => u.ApplicationUserId == userId, includeProperties: "ApplicationUser");
            }
            switch (status)
            {
                case "pending":
                    objOrderHeaders = objOrderHeaders.Where(u => u.PaymentStatus == SD.PaymentStatusPending);
                    break;
                case "inprocess":
                    objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == SD.StatusInProcess);
                    break;
                case "completed":
                    objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == SD.StatusShipped);
                    break;
                case "approved":
                    objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == SD.StatusApproved);
                    break;
                default:
                    break;
            }
            return Json(new { data = objOrderHeaders });
        }
        #endregion
    }
}