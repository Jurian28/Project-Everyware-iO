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
            Event eventModel = new Event();
            List<Event> events = new List<Event>();
            events.Add(eventModel);

            return View(events);
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
