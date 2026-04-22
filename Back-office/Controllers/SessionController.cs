using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace Back_office.Controllers
{
    [Route("{eventId}/sessions")]
    public class SessionController : Controller
    {

        private readonly HttpClient client;

        public SessionController(IHttpClientFactory httpClientFactory)
        {
            client = httpClientFactory.CreateClient("DatabaseApi");
        }

        [HttpGet]
        public async Task<IActionResult> Index(int eventId)
        {
            ViewData["EventId"] = eventId;
            HttpResponseMessage response = await client.GetAsync($"{eventId}/sessions");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Er is iets misgegaan bij het ophalen van de sessies.";
                return RedirectToAction("Index", "Home");
            }

            List<SessionDTO> sessions = (await response.Content.ReadFromJsonAsync<ApiResponse<List<SessionDTO>>>()).Data ?? new List<SessionDTO>();

            SessionListDto dto = new SessionListDto
            {
                EventId = eventId,
                Sessions = sessions
            };
            return View(dto);
        }

        [HttpGet("add")]
        public async Task<IActionResult> AddSession(int eventId)
        {
            ViewData["EventId"] = eventId;
            HttpResponseMessage response = await client.GetAsync($"{eventId}/sessions/getAdd");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Er is iets misgegaan bij het ophalen van de bruikbare ruimtes, tags of sprekers.";
                return RedirectToAction("Index");
            }

            CUSessionDTO emptySession = (await response.Content.ReadFromJsonAsync<ApiResponse<CUSessionDTO>>()).Data ?? new CUSessionDTO();

            return View("SessionForm", emptySession);
        }

        [HttpGet("{sessionId}/edit")]
        public async Task<IActionResult> EditSession(int eventId, int sessionId)
        {
            ViewData["EventId"] = eventId;
            HttpResponseMessage response = await client.GetAsync($"{eventId}/sessions/{sessionId}/edit");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Er is iets misgegaan bij het ophalen van de sessie.";
                return RedirectToAction("Index");
            }

            CUSessionDTO session = (await response.Content.ReadFromJsonAsync<ApiResponse<CUSessionDTO>>()).Data ?? new CUSessionDTO();
            return View("SessionForm", session);
        }

        [HttpPost("save")]
        public async Task<IActionResult> HandleSubmit(int eventId, SessionDTO session, List<string> selectedTagTitles)
        {
            session.Tags = selectedTagTitles
                .Select(t => new SessionTagDTO { Title = t, EventId = eventId })
                .ToList()
                ?? new List<SessionTagDTO>();

            HttpResponseMessage response = await client.PostAsJsonAsync($"{eventId}/sessions/save", session);

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = $"Kon de sessie niet opslaan: {response.ReasonPhrase}";
                return RedirectToAction("Index", new { eventId = eventId });
            }

            return RedirectToAction("Index", new { eventId = eventId });
        }

        [HttpPost("{sessionId}/delete")]
        public async Task<IActionResult> DeleteSession(int eventId, int sessionId)
        {
            HttpResponseMessage response = await client.DeleteAsync($"{eventId}/sessions/{sessionId}/delete");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Er is iets mis gegaan bij het verwijderen van de sessie.";
                return RedirectToAction("Index", new { eventId = eventId });
            }

            return RedirectToAction("Index", new { eventId = eventId });
        }
    }
}