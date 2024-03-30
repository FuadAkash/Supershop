using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Supershop.Data;
using Supershop.Models;
using System.Collections.Generic;
using System.Linq;

namespace Supershop.Controllers
{
    public class itemsController : Controller
    {
        private readonly ApplicationDbContext _db;

        public itemsController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            List<items> objitemList = _db.items.ToList();
            return View(objitemList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(items obj)
        {
            if (ModelState.IsValid)
            {
                _db.items.Add(obj);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }

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
        public IActionResult Edit(items obj)
        {
            if (ModelState.IsValid)
            {
                _db.items.Update(obj);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}
