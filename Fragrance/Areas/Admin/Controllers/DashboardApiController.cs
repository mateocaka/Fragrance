using Fragrance.DataAccess.Repository.IRepository;
using Fragrance.Models;
using Fragrance.Utility;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Fragrance.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardApiController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private const string ApprovedStatus = SD.StatusApproved;
        private const string ShippedStatus = SD.StatusShipped;

        public DashboardApiController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("total-users")]
        public async Task<IActionResult> GetTotalUsers()
        {
            var users = await _unitOfWork.ApplicationUser.GetAllAsync();
            return Ok(

              users.Count());
           
        }

        [HttpGet("monthly-sales")]
        public async Task<IActionResult> GetMonthlySales()
        {
            var currentMonth = DateTime.Now;
            var lastMonth = currentMonth.AddMonths(-1);

            Expression<Func<OrderHeader, bool>> currentFilter = o =>
                (o.OrderStatus == ApprovedStatus || o.OrderStatus == ShippedStatus) && // Both statuses
                o.OrderDate.Year == currentMonth.Year &&
                o.OrderDate.Month == currentMonth.Month;

            Expression<Func<OrderHeader, bool>> lastFilter = o =>
                (o.OrderStatus == ApprovedStatus || o.OrderStatus == ShippedStatus) && // Both statuses
                o.OrderDate.Year == lastMonth.Year &&
                o.OrderDate.Month == lastMonth.Month;

            var currentMonthSales = (await _unitOfWork.OrderHeader.GetAllAsync(currentFilter))
                .Sum(o => o.OrderTotal);

            var lastMonthSales = (await _unitOfWork.OrderHeader.GetAllAsync(lastFilter))
                .Sum(o => o.OrderTotal);

            var percentageChange = lastMonthSales == 0 ? 0 :
                ((currentMonthSales - lastMonthSales) / lastMonthSales) * 100;

            return Ok(new
            {
                TotalSales = currentMonthSales,
                PercentageChange = percentageChange,
                StatusesUsed = new[] { ApprovedStatus, ShippedStatus } // Explicitly note usage
            });
        }

        [HttpGet("total-orders")]
        public async Task<IActionResult> GetTotalOrders()
        {
            var orders = await _unitOfWork.OrderHeader.GetAllAsync(o =>
                o.OrderStatus == ApprovedStatus || o.OrderStatus == ShippedStatus); // Both statuses
            return Ok(

                orders.Count());
               
            
        }

        [HttpGet("products")]
        public async Task<IActionResult> GetProducts()
        {
            var orderHeaders = await _unitOfWork.OrderHeader
                .GetAllAsync(o => o.OrderStatus == ApprovedStatus || o.OrderStatus == ShippedStatus); // Both statuses

            var orderIds = orderHeaders.Select(o => o.Id);

            var products = (await _unitOfWork.OrderDetail
                    .GetAllAsync(od => orderIds.Contains(od.OrderHeaderId),
                               includeProperties: "Parfume"))
                .GroupBy(od => new { od.ParfumeId, od.Parfume.Name, od.Price })
                .Select(g => new
                {
                    Name = g.Key.Name ?? "Unknown",
                    Quantity = g.Sum(od => od.Count),
                    Price = g.Key.Price,
                    Statuses = new { ApprovedStatus, ShippedStatus } // Include in response
                })
                .ToList();

            return Ok(products);
        }

        [HttpGet("orders-per-month")]
        public async Task<IActionResult> GetOrdersPerMonth()
        {
            var orders = (await _unitOfWork.OrderHeader
                    .GetAllAsync(o => o.OrderStatus == ApprovedStatus || o.OrderStatus == ShippedStatus)) // Both statuses
                .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Count = g.Count(),
                    Statuses = new { ApprovedStatus, ShippedStatus } // Include in response
                })
                .OrderBy(g => g.Year).ThenBy(g => g.Month)
                .ToList();

            return Ok(orders);
        }

        [HttpGet("sales-2025")]
        public async Task<IActionResult> GetSales2025()
        {
            Expression<Func<OrderHeader, bool>> filter = o =>
                (o.OrderStatus == ApprovedStatus || o.OrderStatus == ShippedStatus) && // Both statuses
                o.OrderDate.Year == 2025;

            var sales = (await _unitOfWork.OrderHeader.GetAllAsync(filter))
                .GroupBy(o => o.OrderDate.Month)
                .Select(g => new
                {
                    Month = g.Key,
                    Total = g.Sum(o => o.OrderTotal),
                    Statuses = new { ApprovedStatus, ShippedStatus } // Include in response
                })
                .OrderBy(g => g.Month)
                .ToList();

            return Ok(sales);
        }
    }
}