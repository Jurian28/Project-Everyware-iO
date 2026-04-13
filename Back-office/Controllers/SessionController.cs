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
            HttpResponseMessage response = await client.GetAsync($"{eventId}/sessions");

            if (!response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            List<SessionDTO> sessions = await response.Content.ReadFromJsonAsync<List<SessionDTO>>() ?? new List<SessionDTO>();

            SessionListDto dto = new SessionListDto
            {
                EventId = eventId,
                Sessions = sessions ?? new List<SessionDTO>()
            };
            return View(dto);
        }

        [HttpGet("add")]
        public async Task<IActionResult> AddSession(int eventId)
        {
            HttpResponseMessage response = await client.GetAsync($"{eventId}/sessions/add");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Er is iets misgegaan bij het ophalen van de bruikbare ruimtes/tags/sprekers.";
                return RedirectToAction("Index");
            }

            CUSessionDTO emptySession = await response.Content.ReadFromJsonAsync<CUSessionDTO>() ?? new CUSessionDTO();

            return View("SessionForm", emptySession);
        }

        [HttpGet("{sessionId}/edit")]
        public async Task<IActionResult> EditSession(int eventId, int sessionId)
        {
            HttpResponseMessage response = await client.GetAsync($"{eventId}/sessions/{sessionId}/edit");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Er is iets misgegaan bij het ophalen van de sessie.";
                return RedirectToAction("Index");
            }

            CUSessionDTO session = await response.Content.ReadFromJsonAsync<CUSessionDTO>() ?? new CUSessionDTO();

            return View("SessionForm", session);
        }

        [HttpPost("save")]
        public async Task<IActionResult> HandleSubmit(int eventId, SessionDTO session, List<string> selectedTagTitles)
        {
            session.Tags = selectedTagTitles
                .Select(t => new SessionTagDTO { Title = t, EventId = eventId })
                .ToList()
                ?? new List<SessionTagDTO>();

            Console.WriteLine("Sending to API: " + JsonSerializer.Serialize(session));

            var response = await client.PostAsJsonAsync($"{eventId}/sessions/save", session);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", new { eventId = eventId });
            }

            // FOUTOPSPORING: Lees de error van de API
            var errorContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"API Failure ({response.StatusCode}): {errorContent}");

            TempData["Error"] = $"Kon de sessie niet opslaan: {response.ReasonPhrase}";
            return RedirectToAction("Index", new { eventId = eventId });
        }
    }
}