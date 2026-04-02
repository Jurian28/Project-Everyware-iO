using Back_office.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using System.Reflection;

namespace Back_office.Controllers
{
    [Route("events")]
    public class EventsController : Controller
    {
        public EventsController ()
        {
            
        }

        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpGet]
        [Route("{id}/edit")]
        public IActionResult Edit(int id)
        {
            Event eventModel = new Event();
            eventModel.IdEvent = id;
            eventModel.Title = "Teen Titans Go";
            eventModel.Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.";
            eventModel.Location = "Paris, France";
            eventModel.StartDate = DateTime.Now;
            eventModel.EndDate = DateTime.Now.AddDays(2);
            eventModel.MainColorHex = "#FF5733";
            eventModel.AccentColorHex = "#33C1FF";
            eventModel.LogoPath = "/images/teen-titans-go-logo.png";

            return View(eventModel);
        }

        [HttpPost("store")]
        public async Task<IActionResult> Store(Event eventModel, IFormFile? logoFile) 
        {
            try
            {
                Console.WriteLine($"Received event: {eventModel.Title}");
                Console.WriteLine($"Received file: {logoFile?.FileName}");
                // TODO

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View("Create", eventModel);
            }
        }

        [HttpPost("update")]
        public async Task<IActionResult> Update(Event eventModel, IFormFile? logoFile)
        {
            try
            {
                if(logoFile != null)
                {
                    Console.WriteLine($"Received file: {logoFile.FileName}");
                }
                Console.WriteLine($"Received event: {eventModel.Title}");
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
