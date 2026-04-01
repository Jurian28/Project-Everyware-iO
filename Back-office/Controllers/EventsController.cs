using Back_office.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

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
        [Route("{id}")]
        public IActionResult Show(int id)
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
        [Route("edit/{id}")]
        public IActionResult Edit(int id)
        {
            return View();
        }

        [HttpPost("store")]
        public async Task<IActionResult> Store(Event eventModel, IFormFile logoFile) 
        {
            try
            {
                Console.WriteLine($"Received event: {eventModel.Title}");
                Console.WriteLine($"Received file: {logoFile.FileName}");
                // TODO

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View("Create", eventModel);
            }
        }

        [HttpPost("update")]
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
