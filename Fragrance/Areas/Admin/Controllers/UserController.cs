using Fragrance.Data;
using Fragrance.DataAccess.Repository;
using Fragrance.DataAccess.Repository.IRepository;
using Fragrance.Models;
using Fragrance.Models.ViewModels;
using Fragrance.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data.Entity;
using System.Security.Claims;

namespace Fragrance.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<Microsoft.AspNetCore.Identity.IdentityUser> _userManager;
        public UserController(ApplicationDbContext db, UserManager<Microsoft.AspNetCore.Identity.IdentityUser> userManager)
        {
            _userManager = userManager;
            
            _db = db;
            
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult RoleManagment(string userId)
        {
            var RoleID = _db.UserRoles.FirstOrDefault(u => u.UserId == userId).RoleId;

            RoleManagmentVM RoleVM = new RoleManagmentVM()
            {
                ApplicationUser = _db.applicationUsers.Include(u=>u.Company).FirstOrDefault(u => u.Id == userId),
                RoleList = _db.Roles.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Name
                }),
                Companies = _db.Companies.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
            };  
            RoleVM.ApplicationUser.Role = _db.Roles.FirstOrDefault(u => u.Id == RoleID).Name;
            return View(RoleVM);
        }
        [HttpPost]
        public async Task<IActionResult> RoleManagment(RoleManagmentVM roleManagmentVM)
        {
          
            var applicationUser = _db.applicationUsers.FirstOrDefault(u => u.Id == roleManagmentVM.ApplicationUser.Id);
            if (applicationUser == null) return NotFound();

         
            var RoleID = _db.UserRoles.FirstOrDefault(u => u.UserId == applicationUser.Id)?.RoleId;
            string oldRole = _db.Roles.FirstOrDefault(u => u.Id == RoleID)?.Name;

            if (roleManagmentVM.ApplicationUser.Role != oldRole)
            {
               
                if (roleManagmentVM.ApplicationUser.Role == SD.Role_Company)
                {
                    applicationUser.CompanyId = roleManagmentVM.ApplicationUser.CompanyId;
                }
                else if (oldRole == SD.Role_Company)
                {
                    applicationUser.CompanyId = null;
                }
                _db.SaveChanges();
              
               await _userManager.RemoveFromRoleAsync(applicationUser, oldRole);                               
               await _userManager.AddToRoleAsync(applicationUser, roleManagmentVM.ApplicationUser.Role);
            }

            return RedirectToAction("Index");
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<ApplicationUser> objUserList = _db.applicationUsers
                .Include(u => u.Company)  // This should load the company
                .ToList();

            var userRoles = _db.UserRoles.ToList();
            var roles = _db.Roles.ToList();

            foreach (var user in objUserList)
            {
                var roleId = userRoles.FirstOrDefault(u => u.UserId == user.Id)?.RoleId;
                user.Role = roles.FirstOrDefault(r => r.Id == roleId)?.Name ?? "None";

                // Ensure company is properly initialized if null
                if (user.Company == null && user.CompanyId.HasValue)
                {
                    user.Company = _db.Companies.FirstOrDefault(c => c.Id == user.CompanyId);
                }
            }

            return Json(new { data = objUserList });
        }

        [HttpPost]
        public IActionResult LockUnlock([FromBody] string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Json(new { success = false, message = "Invalid user ID" });
            }

            var userFromDb = _db.applicationUsers.FirstOrDefault(u => u.Id == id);
            if (userFromDb == null)
            {
                return Json(new { success = false, message = "User not found" });
            }

            if (userFromDb.LockoutEnd != null && userFromDb.LockoutEnd > DateTime.Now)
            {
                userFromDb.LockoutEnd = DateTime.Now; // Unlock
            }
            else
            {
                userFromDb.LockoutEnd = DateTime.Now.AddYears(1000); // Lock
            }

            _db.SaveChanges();
            return Json(new { success = true, message = "Operation successful" });
        }
        #endregion
    }
}