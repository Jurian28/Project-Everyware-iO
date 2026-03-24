using Back_office.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace Back_office.Controllers
{
    [Route("events")]
    public class EventsController : Controller
    {
        private readonly UserManager<User> _userManager;

        public EventsController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Show()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Edit()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Store(Event eventModel) 
        {
            try
            {
                // TODO

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View("Create", eventModel);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Update(Event eventModel)
        {
            try
            {
                // TODO

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View("Edit", eventModel);
            }
        }
    }
}
