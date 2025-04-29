using Fragrance.Data;
using Fragrance.DataAccess.Repository.IRepository;
using Fragrance.Models;
using Fragrance.Models.ViewModels;
using Fragrance.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data.Entity;
using System.Security.Claims;

namespace Fragrance.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _db;

        public UserController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            List<ApplicationUser> objUserList = _db.applicationUsers
                .Include(u => u.company)
                .ToList();

            var userRoles = _db.UserRoles.ToList();
            var roles = _db.Roles.ToList();

            foreach (var user in objUserList)
            {
                var roleId = userRoles.FirstOrDefault(u => u.UserId == user.Id)?.RoleId;
                user.Role = roles.FirstOrDefault(r => r.Id == roleId)?.Name ?? "None";
                user.company ??= new() { Name = "" };
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
    }
}