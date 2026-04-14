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

            ApiResponse<List<SessionDTO>> sessionsApiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<SessionDTO>>>() ?? new ApiResponse<List<SessionDTO>>();

            List<SessionDTO> sessions = sessionsApiResponse.Data;

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
            HttpResponseMessage response = await client.GetAsync($"{eventId}/sessions/getAdd");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Er is iets misgegaan bij het ophalen van de bruikbare ruimtes/tags/sprekers.";
                return RedirectToAction("Index");
            }

            ApiResponse<CUSessionDTO> emptySessionApiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<CUSessionDTO>>() ?? new ApiResponse<CUSessionDTO>();

            CUSessionDTO emptySession = emptySessionApiResponse.Data;

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

            ApiResponse<CUSessionDTO> sessionApiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<CUSessionDTO>>() ?? new ApiResponse<CUSessionDTO>();

            CUSessionDTO session = sessionApiResponse.Data;

            return View("SessionForm", session);
        }

        [HttpPost("save")]
        public async Task<IActionResult> HandleSubmit(int eventId, SessionDTO session, List<string> selectedTagTitles)
        {
            session.Tags = selectedTagTitles
                .Select(t => new SessionTagDTO { Title = t, EventId = eventId })
                .ToList()
                ?? new List<SessionTagDTO>();

            var response = await client.PostAsJsonAsync($"{eventId}/sessions/save", session);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", new { eventId = eventId });
            }

            TempData["Error"] = $"Kon de sessie niet opslaan: {response.ReasonPhrase}";
            return RedirectToAction("Index", new { eventId = eventId });
        }
    }
}