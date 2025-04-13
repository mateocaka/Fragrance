using Fragrance.DataAccess.Repository.IRepository;
using Fragrance.Models;
using Fragrance.Models.ViewModels;
using Fragrance.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Fragrance.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            List<OrderHeader> objOrderHeaders = _unitOfWork.OrderHeader
                .GetAll(includeProperties: "ApplicationUser")
                .ToList();

            var data = objOrderHeaders.Select(o => new {
                id = o.Id,
                name = o.Name,
                phoneNumber = o.PhoneNumber,
                applicationUser = o.ApplicationUser != null ? new
                {
                    email = o.ApplicationUser.Email,
                    isAdmin = User.IsInRole(SD.Role_Admin)
                } : null,
                orderStatus = o.OrderStatus,
                paymentStatus = o.PaymentStatus,
                orderTotal = o.OrderTotal,
                orderDate = o.OrderDate.ToString("MM/dd/yyyy hh:mm tt")
            });

            return Json(new { data = data });
        }

        public IActionResult Details(int orderId)
        {
            OrderVM orderVM = new()
            {
                OrderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == orderId, includeProperties: "ApplicationUser"),
                OrderDetail = _unitOfWork.OrderDetail.GetAll(u => u.OrderHeaderId == orderId, includeProperties: "Parfume")
            };

            return View(orderVM);
        }

        [HttpPost]
        public IActionResult UpdateOrderStatus(int orderId, string status)
        {
            var orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == orderId);
            if (orderHeader == null)
            {
                return Json(new { success = false, message = "Order not found" });
            }

          
            if (!User.IsInRole(SD.Role_Admin) || orderHeader.OrderStatus != SD.StatusApproved)
            {
                if (status == SD.StatusApproved)
                {
                    var orderDetails = _unitOfWork.OrderDetail.GetAll(od => od.OrderHeaderId == orderId, includeProperties: "Parfume");
                    foreach (var detail in orderDetails)
                    {
                        var product = _unitOfWork.Parfume.Get(p => p.ParfumeId == detail.ParfumeId);
                        if (product.Quantity < detail.Count )
                        {
                            return Json(new
                            {
                                success = false,
                                message = $"Not enough stock for {product.Name}. Only {product.Quantity} available."
                            });
                        }
                    }

                   
                    foreach (var detail in orderDetails)
                    {
                        var product = _unitOfWork.Parfume.Get(p => p.ParfumeId == detail.ParfumeId);
                        product.Quantity -= detail.Count;
                        _unitOfWork.Parfume.Update(product);
                    }
                }
            }

            orderHeader.OrderStatus = status;
            if (status == SD.StatusApproved && orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
            {
                orderHeader.PaymentStatus = SD.PaymentStatusApproved;
            }
            else if (status == SD.StatusCancelled)
            {
                orderHeader.PaymentStatus = SD.PaymentStatusRejected;
            }

            _unitOfWork.OrderHeader.Update(orderHeader);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Order status updated successfully" });
        }
    }
}