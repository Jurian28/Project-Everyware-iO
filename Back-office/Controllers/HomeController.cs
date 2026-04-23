using Back_office.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;

namespace Back_office.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.Events = new[]
            {
                new SelectListItem { Value = "1", Text = "Event 1" },
                new SelectListItem { Value = "2", Text = "Event 2" },
                new SelectListItem { Value = "3", Text = "Event 3" }
            };
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
