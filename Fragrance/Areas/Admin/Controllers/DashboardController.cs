using Fragrance.DataAccess.Repository.IRepository;
using Fragrance.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Fragrance.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public DashboardController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                
                var users = await _unitOfWork.ApplicationUser.GetAllAsync();
                var totalUsers = users.Count();

              
                var currentMonth = DateTime.Now;
                var lastMonth = currentMonth.AddMonths(-1);

                Expression<Func<OrderHeader, bool>> currentFilter =
                    o => o.OrderDate.Year == currentMonth.Year &&
                         o.OrderDate.Month == currentMonth.Month;

                Expression<Func<OrderHeader, bool>> lastFilter =
                    o => o.OrderDate.Year == lastMonth.Year &&
                         o.OrderDate.Month == lastMonth.Month;

                var currentMonthSales = (await _unitOfWork.OrderHeader.GetAllAsync(currentFilter))
                    .Sum(o => o.OrderTotal);

                var lastMonthSales = (await _unitOfWork.OrderHeader.GetAllAsync(lastFilter))
                    .Sum(o => o.OrderTotal);

                var percentageChange = lastMonthSales == 0 ? 0 :
                    ((currentMonthSales - lastMonthSales) / lastMonthSales) * 100;



                return View();
            }
            catch (Exception ex)
            {
                return View("Error", new { message = $"Failed to load dashboard: {ex.Message}" });
            }
        }
    }
}