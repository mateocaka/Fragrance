using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Claims;
using Fragrance.DataAccess.Repository.IRepository;
using Fragrance.Models;
using Fragrance.Models.ViewModels;
using Fragrance.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Fragrance.Areas.Costumer.Controllers
        
{
    [Area("Costumer")]

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index(string search, string gender, string brand, string rating)
        {
            var allPerfumes = _unitOfWork.Parfume.GetAll().ToList();

          
            var filteredPerfumes = allPerfumes.AsEnumerable();

            if (!string.IsNullOrEmpty(search))
            {
                filteredPerfumes = filteredPerfumes.Where(p =>
                    p.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    p.Author.Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(gender))
            {
                filteredPerfumes = filteredPerfumes.Where(p => p.Gender == gender);
            }

            if (!string.IsNullOrEmpty(brand))
            {
                filteredPerfumes = filteredPerfumes.Where(p =>
                    p.Author.Equals(brand, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(rating) && double.TryParse(rating.TrimEnd('+'), out double minRating))
            {
                filteredPerfumes = filteredPerfumes.Where(p => p.Rating >= minRating);
            }

           
            ViewBag.OriginalData = allPerfumes;
            ViewBag.Genders = new List<string> { "Male", "Female", "Unisex" };
            ViewBag.Brands = allPerfumes
                .Select(p => p.Author)
                .Distinct()
                .OrderBy(a => a)
                .ToList();

            var ratingsList = new List<string> {"1", "2", "3", "4","5" };
            ViewBag.Ratings = ratingsList;
    
            ViewBag.CurrentFilters = new
            {
                Search = search,
                Gender = gender,
                Brand = brand,
                Rating = rating
            };
            if (filteredPerfumes!=null)
            {
                return View(filteredPerfumes);
            }
            
            return View(filteredPerfumes);
        }
        public IActionResult Details(int parfumeid)
        {        
            ShoppingCart cart = new()
            {
                Parfume = _unitOfWork.Parfume.Get(u => u.ParfumeId == parfumeid),
                Count = 1,
                ParfumeId = parfumeid,
            };


            return View(cart);
        }
        public IActionResult SoldOUT(int parfumeid)
        {
            ShoppingCart cart = new()
            {
                Parfume = _unitOfWork.Parfume.Get(u => u.ParfumeId == parfumeid),
                Count = 1,
                ParfumeId = parfumeid,
            };


            return View(cart);
        }
        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            shoppingCart.ApplicationUserId = userId;
            var parfume = _unitOfWork.Parfume.Get(u => u.ParfumeId == shoppingCart.ParfumeId);

            ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.Get(u =>
                u.ApplicationUserId == userId && u.ParfumeId == shoppingCart.ParfumeId);
            if (shoppingCart.Count > parfume.Quantity)
            {
                TempData["Notification"] = "error|Not enough stock available";
                return RedirectToAction(nameof(Index));
            }
            if (cartFromDb != null)
            {
                
                cartFromDb.Count += shoppingCart.Count;
               
                _unitOfWork.ShoppingCart.Update(cartFromDb);
                _unitOfWork.Save();
            }
            else
            {
                //add cart and also add session
               
                _unitOfWork.ShoppingCart.Add(shoppingCart);
                _unitOfWork.Save();
                HttpContext.Session.SetInt32(SD.SessionCart, 
                _unitOfWork.ShoppingCart.GetAll(u =>
                u.ApplicationUserId == userId).Count());
            }
            TempData["Notification"] = "success|Cart update successfully";


            _unitOfWork.Save();
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
