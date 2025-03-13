using Fragrance.Data;
using Fragrance.Models;
using Fragrance.Models.ViewModels;
using Fragrance.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Fragrance.Utility;


namespace Fragrance.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ParfumeController : Controller
    {
      
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ParfumeController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            List<Parfume> objProductList = _unitOfWork.Parfume.GetAll().ToList();

            return View(objProductList);
        }

        public IActionResult Upsert(int? id)
        {
            ParfumeVM productVm = new()
            {
                ParfumeList = _unitOfWork.Parfume.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.ParfumeId.ToString()
                }),
                Parfum = new Parfume()
            };

            if (id == null || id == 0)
            {
               
                return View(productVm);
            }
            else
            {
                
                productVm.Parfum = _unitOfWork.Parfume.Get(u => u.ParfumeId == id);
                return View(productVm);
            }
        }

        [HttpPost]
        public IActionResult Upsert(ParfumeVM productVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;

                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"images\product");

                    if (!string.IsNullOrEmpty(productVM.Parfum.ImgUrl))
                    {
                        var oldImagePath = Path.Combine(wwwRootPath, productVM.Parfum.ImgUrl.TrimStart('\\'));

                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    productVM.Parfum.ImgUrl = @"\images\product\" + fileName;
                }

                if (productVM.Parfum.ParfumeId == 0)
                {
                    _unitOfWork.Parfume.Add(productVM.Parfum);
                }
                else
                {
                    _unitOfWork.Parfume.Update(productVM.Parfum);
                }

                _unitOfWork.Save();
                TempData["success"] = "Product created/updated successfully";
                return RedirectToAction("Index");
            }
            else
            {
                productVM.ParfumeList = _unitOfWork.Parfume.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.ParfumeId.ToString()
                });

                return View(productVM);
            }
        }


        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Parfume categoryFromDb = _unitOfWork.Parfume.Get(u => u.ParfumeId == id);
         
            if (categoryFromDb == null)
            {
                return NotFound();
            }

            return View(categoryFromDb);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            Parfume? obj = _unitOfWork.Parfume.Get(u => u.ParfumeId == id);
            if (obj == null)
            {
                return NotFound();
            }
            _unitOfWork.Parfume.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Category delete successfully";
            return RedirectToAction("Index");


        }



        #region API CALLS 
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Parfume> objProductList = _unitOfWork.Parfume.GetAll(includeProperties: "Price").ToList();
            return Json(new { data = objProductList });
        }


        


        #endregion
    
    }
}

