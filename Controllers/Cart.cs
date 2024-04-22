﻿using Microsoft.AspNetCore.Mvc;
using Supershop.Models;
using Supershop.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Supershop.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;
        private List<CartItem> _cart = new List<CartItem>(); // Simulating a cart stored in memory

        public CartController(ApplicationDbContext db)
        {
            _db = db;
        }
        
        [Authorize]
        public IActionResult Index()
        {
            List<items> itemList = _db.items.ToList();
            return View(itemList);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Search(string searchTerm)
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
    }
}
