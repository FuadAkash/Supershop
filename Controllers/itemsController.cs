using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Supershop.Data;
using Supershop.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;


namespace Supershop.Controllers
{
    public class itemsController : Controller
    {
        private readonly ApplicationDbContext _db;

        private readonly IWebHostEnvironment _webHostEnvironment;

        public itemsController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
        }
        [Authorize]
        public IActionResult Index()
        {
            List<items> objitemList = _db.items.ToList();
            return View(objitemList);
        }
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public IActionResult Create(items obj, IFormFile file)
        {

            if (file != null && file.Length > 0)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                file.CopyTo(new FileStream(filePath, FileMode.Create));
                obj.ImageUrl = uniqueFileName; // Save the file name to the database
            }

            _db.items.Add(obj);
            _db.SaveChanges();
            TempData["success"] = "Item Created Successfully!";
            return RedirectToAction("Index");
        }
        [Authorize]
        public IActionResult Edit(int? Id)
        {
            if (Id==null || Id==0)
            {
                return NotFound();
            }
            items? itemobj = _db.items.Find(Id);
            //items? itemobj1 = _db.items.FirstOrDefault(itemobj=>itemobj.Id==Id);
            //items? itemobj2 = _db.items.Find(Id);

            if (itemobj==null)
            {
                return NotFound();
            }
            
            return View(itemobj);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Edit(items obj, IFormFile file)
        {
            // Retrieve the existing item from the database
            var existingItem = _db.items.FirstOrDefault(item => item.Id == obj.Id);

            if (existingItem == null)
            {
                return NotFound(); // Handle if item not found
            }

            if (file != null && file.Length > 0)
            {
                // Handle the case when a new image is uploaded
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");

                // Delete the previous image file if it exists
                if (!string.IsNullOrEmpty(existingItem.ImageUrl))
                {
                    string previousImagePath = Path.Combine(uploadsFolder, existingItem.ImageUrl);
                    if (System.IO.File.Exists(previousImagePath))
                    {
                        System.IO.File.Delete(previousImagePath);
                    }
                }

                // Generate a unique file name for the new image
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                file.CopyTo(new FileStream(filePath, FileMode.Create));

                // Update the image URL with the new file name
                existingItem.ImageUrl = uniqueFileName;
            }

            // Update other properties of the existing item
            existingItem.Name = obj.Name;
            existingItem.Type = obj.Type;
            existingItem.Count = obj.Count;
            existingItem.location = obj.location;
            existingItem.Price = obj.Price;

            // Update the item in the database
            _db.items.Update(existingItem);
            _db.SaveChanges();

            TempData["success"] = "Item Edited Successfully!";
            return RedirectToAction("Index");
        }
        [Authorize]
        public IActionResult Delete(int? Id)
        {
            if (Id == null || Id == 0)
            {
                return NotFound();
            }
            items? itemobj = _db.items.Find(Id);
            //items? itemobj1 = _db.items.FirstOrDefault(itemobj=>itemobj.Id==Id);
            //items? itemobj2 = _db.items.Find(Id);

            if (itemobj == null)
            {
                return NotFound();
            }

            return View(itemobj);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize]
        public IActionResult DeletePost(int? Id)
        {

            items? obj = _db.items.Find(Id);
            //items? itemobj1 = _db.items.FirstOrDefault(itemobj=>itemobj.Id==Id);
            //items? itemobj2 = _db.items.Find(Id);

            if (obj == null)
            {
                return NotFound();
            }

            _db.items.Remove(obj);
            _db.SaveChanges();
            _db.SaveChanges();
            TempData["success"] = "Item Deleted Successfully!";
            return RedirectToAction("Index");
        }
    }
}
