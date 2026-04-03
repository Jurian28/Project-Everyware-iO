using Back_office.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Back_office.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            ViewData["EventId"] = 1;
            return View();
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
