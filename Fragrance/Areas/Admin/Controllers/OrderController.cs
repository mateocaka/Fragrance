using Fragrance.DataAccess.Repository.IRepository;
using Fragrance.Models;
using Fragrance.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fragrance.Areas.Admin.Controllers
{
    [Area("admin")]
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

            // Konverto në objekt anonim për të shmangur referencat e panevojshme
            var data = objOrderHeaders.Select(o => new {
                id = o.Id,
                name = o.Name,
                phoneNumber = o.PhoneNumber,
                applicationUser = o.ApplicationUser != null ? new { email = o.ApplicationUser.Email } : null,
                orderStatus = o.OrderStatus,
                orderTotal = o.OrderTotal
            });

            return Json(new { data = data });
        }
        
    }
}
