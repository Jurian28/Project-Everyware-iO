using Microsoft.AspNetCore.Mvc;

namespace Back_office.Controllers
{
    [Route("{eventId}/sessions")]
    public class SessionsController : Controller
    {
        [HttpGet]
        public IActionResult Index(int eventId)
        {
            ViewBag.eventId = eventId;
            return View();
        }
    }
}