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

       public IActionResult Index()
        {
          
            IEnumerable<Parfume> parfumelist = _unitOfWork.Parfume.GetAll();
            return View(parfumelist);
        }

        public IActionResult Detials(int parfumeid)
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
        public IActionResult Detials(ShoppingCart shoppingCart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            shoppingCart.ApplicationUserId = userId;

          
            ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.Get(u =>
                u.ApplicationUserId == userId && u.ParfumeId == shoppingCart.ParfumeId);

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
            TempData["success"] = "Cart update successfully";



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
