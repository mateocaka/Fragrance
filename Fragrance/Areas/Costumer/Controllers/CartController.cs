using Fragrance.DataAccess.Repository;
using Fragrance.DataAccess.Repository.IRepository;
using Fragrance.Models;
using Fragrance.Models.ViewModels;
using Fragrance.Utility;
using Fragrance.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Exchange.WebServices.Data;
using System.Security.Claims;
using Stripe.Checkout;

namespace Fragrance.Areas.Costumer.Controllers
{
    [Area("Costumer")]
    [Authorize]
    public class CartController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }
        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }



        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM = new()
            {
                ShoppingCartsList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId,
                    includeProperties: "Parfume"),
                OrderHeader = new()
            };

            bool outOfStockItemExists = false;

            foreach (var cart in ShoppingCartVM.ShoppingCartsList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);

                if (cart.Parfume.Quantity == 0)
                {
                    outOfStockItemExists = true;
                }
            }

            if (outOfStockItemExists)
            {
                TempData["Notification"] = "warning|Some items in your cart are currently out of stock.";
            }

            return View(ShoppingCartVM);
        }


        public IActionResult Summary()
        {
             var claimsIdentity = (ClaimsIdentity)User.Identity;
            var useId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM = new()
            {
                ShoppingCartsList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == useId,
                includeProperties: "Parfume"),
                OrderHeader = new()

            };

            ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == useId);


            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
            ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
            ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
            ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
            ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;


            foreach (var cart in ShoppingCartVM.ShoppingCartsList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
               
            }
           
            return View(ShoppingCartVM);
        }

        [HttpPost]
        [ActionName("Summary")]
        public IActionResult SummaryPOST()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM.ShoppingCartsList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId,
                includeProperties: "Parfume");


            foreach (var cart in ShoppingCartVM.ShoppingCartsList)
            {
                if (cart.Parfume.Quantity == 0)
                {
                    TempData["Notification"] = $"error|{cart.Parfume.Name} is out of stock. Please remove it from your cart to proceed.";
                    return RedirectToAction(nameof(Index));
                }

                if (cart.Count > cart.Parfume.Quantity)
                {
                    TempData["Notification"] = $"error|Only {cart.Parfume.Quantity} items available for {cart.Parfume.Name}. Please adjust your quantity.";
                    return RedirectToAction(nameof(Index));
                }
            }
            ShoppingCartVM.OrderHeader.OrderDate = System.DateTime.Now;
            ShoppingCartVM.OrderHeader.ApplicationUserId = userId;
               
            //kurr mos e popullo nje entity kur e perodr si navigate property
            ApplicationUser applicationUsers = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);


            foreach (var cart in ShoppingCartVM.ShoppingCartsList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
                
            }

            if(applicationUsers.CompanyId.GetValueOrDefault() == 0) 
            {
                //it is a reguluar customer 
                ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
                ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
            }
            else
            {
                //it is a company user
                ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
                ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusApproved;
            }
            _unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
            _unitOfWork.Save();
            foreach(var cart in ShoppingCartVM.ShoppingCartsList)
            {
                OrderDetail orderDetail = new()
                {
                    ParfumeId = cart.ParfumeId,
                    OrderHeaderId = ShoppingCartVM.OrderHeader.Id,
                    Price = cart.Price,
                    Count = cart.Count
                };
                _unitOfWork.OrderDetail.Add(orderDetail);
                _unitOfWork.Save();
            }
            if (applicationUsers.CompanyId.GetValueOrDefault() == 0)
            {
                //it is a reguluar customer account and we need to capture a payment
                //stripe logic
                var domain = "https://localhost:7046/";
                var options = new SessionCreateOptions
                {
                    SuccessUrl = domain+$"costumer/cart/OrderConfirmation?id={ShoppingCartVM.OrderHeader.Id}",
                    CancelUrl = domain+"costumer/cart/index",
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                };

                foreach(var item in ShoppingCartVM.ShoppingCartsList)
                {
                    var SessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Price * 100),
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Parfume.Name
                            }
                        },
                        Quantity = item.Count
                    };
                    options.LineItems.Add(SessionLineItem);
                }

                var service = new SessionService();
                Session session=service.Create(options);
                _unitOfWork.OrderHeader.UpdateStripePaymentId(ShoppingCartVM.OrderHeader.Id, session.Id,session.PaymentIntentId);
                _unitOfWork.Save();
                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);



            }



            return RedirectToAction(nameof(OrderConfirmation),new {id=ShoppingCartVM.OrderHeader.Id});
        }

        public IActionResult OrderConfirmation(int id)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == id, includeProperties: "ApplicationUser");
            var orderDetails = _unitOfWork.OrderDetail.GetAll(od => od.OrderHeaderId == id, includeProperties: "Parfume");

            // Check stock availability
            foreach (var detail in orderDetails)
            {
                if (detail.Parfume.Quantity < detail.Count)
                {
                    // Handle insufficient stock scenario
                    _unitOfWork.OrderHeader.UpdateStatus(id, SD.StatusCancelled, SD.PaymentStatusRejected);
                    _unitOfWork.Save();

                    TempData["Notification"] = $"error|{detail.Parfume.Name} is no longer available in the requested quantity. Your order has been cancelled.";
                    return RedirectToAction(nameof(Index));
                }
            }
            
            if (orderHeader.PaymentStatus != SD.PaymentStatusDelayedPayment)
            {
                // For immediate payment orders
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);

                if (session.PaymentStatus.ToLower() == "paid")
                {
                    // Only update payment status, keep order status as Pending for admin approval
                    _unitOfWork.OrderHeader.UpdateStripePaymentId(id, session.Id, session.PaymentIntentId);
                    _unitOfWork.OrderHeader.UpdateStatus(id, SD.StatusPending, SD.PaymentStatusApproved);
                    _unitOfWork.Save();
                }
                HttpContext.Session.Clear();
            }
            else
            {
                // For company users with delayed payment
                // Keep status as Pending for admin approval
                _unitOfWork.OrderHeader.UpdateStatus(id, SD.StatusPending, SD.PaymentStatusDelayedPayment);
                _unitOfWork.Save();
            }

            // Don't deduct inventory here - wait for admin approval
            // Clear cart only after successful payment
            List<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCart
                .GetAll(u => u.ApplicationUserId == orderHeader.ApplicationUserId).ToList();
            _unitOfWork.ShoppingCart.RemoveRange(shoppingCarts);
            _unitOfWork.Save();

            return View(id);
        }

        public IActionResult Plus(int cartId)
        {
            var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.ShoppingCartId == cartId, includeProperties: "Parfume");
            
            if (cartFromDb.Parfume.Quantity > 0)
            {
                cartFromDb.Count += 1;
                if (cartFromDb.Count == cartFromDb.Parfume.Quantity)
                {
                    TempData["Notification"] = $"error|Only {cartFromDb.Parfume.Quantity} items available in stock.";

                }
                else
                {
                   cartFromDb.Price = GetPriceBasedOnQuantity(cartFromDb);
                    _unitOfWork.ShoppingCart.Update(cartFromDb);
                    _unitOfWork.Save();
                    TempData["Notification"] = "success|Item quantity increased!";
                }

            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Minus(int cartId)
        {
            var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.ShoppingCartId == cartId);

            if (cartFromDb.Count <= 1)
            {
                _unitOfWork.ShoppingCart.Remove(cartFromDb);
                HttpContext.Session.SetInt32(SD.SessionCart,
                    _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == cartFromDb.ApplicationUserId).Count() - 1);
                TempData["Notification"] = "success|Item removed from cart!";
            }
            else
            {
                cartFromDb.Count -= 1;
                _unitOfWork.ShoppingCart.Update(cartFromDb);
                TempData["Notification"] = "success|Item quantity decreased!";
            }

            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Remove(int cartId)
        {
            var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.ShoppingCartId == cartId);
            _unitOfWork.ShoppingCart.Remove(cartFromDb);
            HttpContext.Session.SetInt32(SD.SessionCart,
                _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == cartFromDb.ApplicationUserId).Count() - 1);
            _unitOfWork.Save();
            TempData["Notification"] = "success|Item removed from cart!";

            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public IActionResult UpdateCart(int cartId, int count)
        {
            var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.ShoppingCartId == cartId, includeProperties: "Parfume");

            if (cartFromDb.Parfume.Quantity >= count)
            {
                cartFromDb.Count = count;
                _unitOfWork.ShoppingCart.Update(cartFromDb);
                _unitOfWork.Save();
                TempData["Notification"] = "success|Cart updated successfully!";
            }
            else
            {
                TempData["Notification"] = $"error|Only {cartFromDb.Parfume.Quantity} items available in stock.";
            }

            return RedirectToAction(nameof(Index));
        }


        //public IActionResult Remove(int cartId)
        //{
        //    var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.ShoppingCartId == cartId);
        //   _unitOfWork.ShoppingCart.Remove(cartFromDb);
        //    HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == cartFromDb.ApplicationUserId).Count() - 1);
        //    _unitOfWork.Save();

        //    return RedirectToAction(nameof(Index));
        //}



        private double GetPriceBasedOnQuantity(ShoppingCart shoppingCart)
        {
            if (shoppingCart.Count <= 50)
            {
                return shoppingCart.Parfume.Price;
            }
            else
            {
                if (shoppingCart.Count <= 1000)
                {
                    return shoppingCart.Parfume.Price50;
                }
                else
                {
                    return shoppingCart.Parfume.Price100;
                }
            }
        }
    }
}
