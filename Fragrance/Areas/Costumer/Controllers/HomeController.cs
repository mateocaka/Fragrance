using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Claims;
using Fragrance.DataAccess.Repository.IRepository;
using Fragrance.Models;
using Fragrance.Models.ViewModels;
using Fragrance.Rrugetimi;
using Fragrance.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

    
        public IActionResult Index(string search, string gender, string brand, string rating, int page = 1)
        {
            // Determine cookie prefix based on authentication status
            string cookiePrefix = User.Identity.IsAuthenticated ? $"_{User.FindFirst(ClaimTypes.NameIdentifier)?.Value}" : "";
            var theme = Request.Cookies[$"UserTheme{cookiePrefix}"] ?? "light";
            var itemsPerPage = int.Parse("10");
            if (string.IsNullOrEmpty(brand) && RouteData.Values["brand"] != null)
            {
                brand = RouteData.Values["brand"].ToString();
            }
            brand = brand ?? HttpContext.GetRouteValue("brand")?.ToString();
            gender = gender ?? HttpContext.GetRouteValue("gender")?.ToString();
            rating = rating ?? HttpContext.GetRouteValue("rating")?.ToString();
            // Pass preferences to the view
            ViewBag.Theme = theme;
            ViewBag.ItemsPerPage = itemsPerPage;

            // Fetch all perfumes
            var allPerfumes = _unitOfWork.Parfume.GetAll().ToList();

            // Apply filters step by step to get current filtered perfumes
            var filteredPerfumes = allPerfumes.AsEnumerable();

            if (!string.IsNullOrEmpty(search))
            {
                filteredPerfumes = filteredPerfumes.Where(p =>
                    p.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    p.Author.Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            // Get min rating if rating filter is applied
            double minRating = 0;
            if (!string.IsNullOrEmpty(rating) && double.TryParse(rating.TrimEnd('+'), out minRating))
            {
                filteredPerfumes = filteredPerfumes.Where(p => p.Rating >= minRating);
            }

            // Get available genders based on current filters (excluding gender itself)
            var availableGenders = allPerfumes
                .Where(p =>
                    (string.IsNullOrEmpty(search) ||
                     p.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                     p.Author.Contains(search, StringComparison.OrdinalIgnoreCase)) &&
                    (string.IsNullOrEmpty(brand) || p.Author.Equals(brand, StringComparison.OrdinalIgnoreCase)) &&
                    (string.IsNullOrEmpty(rating) || p.Rating >= minRating))
                .Select(p => p.Gender)
                .Distinct()
                .OrderBy(g => g)
                .ToList();

            // Get available brands based on current filters (excluding brand itself)
            var availableBrands = allPerfumes
                .Where(p =>
                    (string.IsNullOrEmpty(search) ||
                     p.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                     p.Author.Contains(search, StringComparison.OrdinalIgnoreCase)) &&
                    (string.IsNullOrEmpty(gender) || p.Gender == gender) &&
                    (string.IsNullOrEmpty(rating) || p.Rating >= minRating))
                .Select(p => p.Author)
                .Distinct()
                .OrderBy(a => a)
                .ToList();

            // Get available ratings based on current filters (excluding rating itself)
            var availableRatings = allPerfumes
                .Where(p =>
                    (string.IsNullOrEmpty(search) ||
                     p.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                     p.Author.Contains(search, StringComparison.OrdinalIgnoreCase)) &&
                    (string.IsNullOrEmpty(gender) || p.Gender == gender) &&
                    (string.IsNullOrEmpty(brand) || p.Author.Equals(brand, StringComparison.OrdinalIgnoreCase)))
                .Select(p => Math.Floor(p.Rating).ToString())
                .Distinct()
                .OrderBy(r => r)
                .ToList();

            // Now apply gender and brand filters to the main query
            if (!string.IsNullOrEmpty(gender))
            {
                filteredPerfumes = filteredPerfumes.Where(p => p.Gender == gender);
            }

            if (!string.IsNullOrEmpty(brand))
            {
                filteredPerfumes = filteredPerfumes.Where(p => p.Author.Equals(brand, StringComparison.OrdinalIgnoreCase));
            }

            // Apply pagination
            var totalItems = filteredPerfumes.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)itemsPerPage);
            var paginatedPerfumes = filteredPerfumes.Skip((page - 1) * itemsPerPage).Take(itemsPerPage).ToList();

            // Pass pagination and filter data to the view
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.OriginalData = allPerfumes;
            ViewBag.AllGenders = new List<string> { "Male", "Female", "Unisex" };
            ViewBag.AllBrands = allPerfumes
                .Select(p => p.Author)
                .Distinct()
                .OrderBy(a => a)
                .ToList();

            var allRatings = new List<string> { "1", "2", "3", "4", "5" };
            ViewBag.AllRatings = allRatings;

            // Pass available options based on current filters
            ViewBag.AvailableGenders = availableGenders;
            ViewBag.AvailableBrands = availableBrands;
            ViewBag.AvailableRatings = availableRatings;

            ViewBag.CurrentFilters = new
            {
                Search = search,
                Gender = gender,
                Brand = brand,
                Rating = rating
            };

            return View(paginatedPerfumes);
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
            if (parfume == null)
            {
                TempData["Notification"] = "error|Perfume not found";
                return RedirectToAction(nameof(Index));
            }

            // Set default size to 30ml if not specified
            if (shoppingCart.Size == 0)
            {
                shoppingCart.Size = 30;
            }

            // Check stock for the selected size
            int availableStock = shoppingCart.Size switch
            {
                30 => parfume.Size30,
                50 => parfume.Size50,
                100 => parfume.Size100,
                _ => 0
            };

            if (shoppingCart.Count > availableStock)
            {
                TempData["Notification"] = "error|Not enough stock available for the selected size";
                return RedirectToAction(nameof(Index));
            }

            // Check if a cart item exists for the same user, perfume, and size
            ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.Get(u =>
                u.ApplicationUserId == userId &&
                u.ParfumeId == shoppingCart.ParfumeId &&
                u.Size == shoppingCart.Size);

            if (cartFromDb != null)
            {
                // Update existing cart item
                cartFromDb.Count += shoppingCart.Count;
                _unitOfWork.ShoppingCart.Update(cartFromDb);
                _unitOfWork.Save();
            }
            else
            {
                // Add new cart item
                _unitOfWork.ShoppingCart.Add(shoppingCart);
                _unitOfWork.Save();
                // Update session cart count
                HttpContext.Session.SetInt32(SD.SessionCart,
                    _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId).Count());
            }

            TempData["Notification"] = "success|Cart updated successfully";
            return RedirectToAction(nameof(Index));
        }
        public IActionResult SetPreferences(string theme, int itemsPerPage)
        {
            var options = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(30),
                IsEssential = true,
                SameSite = SameSiteMode.Lax,
                Secure = true
            };

            // Set cookies with user-specific prefix if authenticated
            string cookiePrefix = User.Identity.IsAuthenticated ? $"_{User.FindFirst(ClaimTypes.NameIdentifier)?.Value}" : "";
            Response.Cookies.Append($"UserTheme{cookiePrefix}", theme, options);
            Response.Cookies.Append($"ItemsPerPage{cookiePrefix}", itemsPerPage.ToString(), options);

            return RedirectToAction("Index");
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