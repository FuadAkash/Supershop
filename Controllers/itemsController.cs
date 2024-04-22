using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Supershop.Data;
using Supershop.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using Supershop.Authorization;


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

        [HttpPost]
        [Authorize]
        public IActionResult Index(string searchTerm)
        {
            List<items> searchResults;
            if (!string.IsNullOrEmpty(searchTerm))
            {
                // Perform search in the database based on the provided search term
                searchResults = _db.items
                    .Where(item => item.Name.Contains(searchTerm) || item.Type.Contains(searchTerm))
                    .ToList();
            }
            else
            {
                searchResults = new List<items>(); // Empty list if no search term provided
            }

            return View("Index", searchResults);

        }

        [OfficerAuthorization]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [OfficerAuthorization]
        public IActionResult Create(items obj, IFormFile file)
        {
            if (!ModelState.IsValid)
            {
                // There are validation errors, so return to the form page with validation errors
                return View(obj);
            }

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


        [OfficerAuthorization]
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
        [OfficerAuthorization]
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
        
        [OfficerAuthorization]
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
        [OfficerAuthorization]
        public IActionResult DeletePost(int? Id)
        {

            items? obj = _db.items.Find(Id);
            //items? itemobj1 = _db.items.FirstOrDefault(itemobj=>itemobj.Id==Id);
            //items? itemobj2 = _db.items.Find(Id);

            if (obj == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(obj.ImageUrl))
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                string imagePath = Path.Combine(uploadsFolder, obj.ImageUrl);
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            _db.items.Remove(obj);
            _db.SaveChanges();
            _db.SaveChanges();
            TempData["success"] = "Item Deleted Successfully!";
            return RedirectToAction("Index");
        }

        public IActionResult AddToCart()
        {
            List<items> objitemList = _db.items.ToList();
            return View(objitemList);
        }

        [HttpPost]
        public IActionResult AddToCart(int itemId, int quantity)
        {
            // Retrieve the item from the database
            var item = _db.items.Find(itemId);

            if (item == null)
            {
                return NotFound(); // Handle if item not found
            }

            // Assuming you have a Cart model with appropriate properties
            var cartItem = new items
            {
                Id = itemId,
                Count = quantity,
                // Set other properties as needed
            };

            _db.items.Add(cartItem);
            _db.SaveChanges();

            // Optionally, you can return a JSON response indicating success
            return Json(new { success = true });
        }

    }
}
