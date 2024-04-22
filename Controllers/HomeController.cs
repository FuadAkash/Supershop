using Microsoft.AspNetCore.Mvc;
using Supershop.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Supershop.Models;
using Supershop.Data;

namespace Supershop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult Index()
        {
            if (Request.Cookies["UserUnauthorized"] == "true")
            {
                TempData["error"] = "User Unauthorized!";
                Response.Cookies.Delete("UserUnauthorized");
            }
            List<items> objitemList = _db.items.ToList();
            return View(objitemList);
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
