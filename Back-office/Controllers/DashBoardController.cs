using Microsoft.AspNetCore.Mvc;

namespace Back_office.Controllers;

[Route("{eventId}/[controller]")]
public class DashBoardController : Controller
{
    public IActionResult Index(int eventId)
    {
        ViewData["EventId"] = eventId;
        return View();
    }

    [HttpGet("{sessionId}")]
    public IActionResult GetSessionDetail(int eventId, int sessionId)
    {
        ViewData["EventId"] = eventId;
        ViewData["SessionId"] = sessionId;

        return View("SessionDetails");
    }
}

