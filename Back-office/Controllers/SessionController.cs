using Microsoft.AspNetCore.Mvc;

namespace Back_office.Controllers
{
    [Route("{eventId}/sessions")]
    public class SessionController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly HttpClient client;

        public SessionController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            client = _httpClientFactory.CreateClient("DatabaseApi");
        }

        [HttpGet]
        public async Task<IActionResult> Index(int eventId)
        {
            var response = await client.GetAsync($"{eventId}/sessions");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var sessions = await response.Content.ReadFromJsonAsync<List<SessionDTO>>();

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
            var response = await client.GetAsync($"{eventId}/sessions/add");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var emptySession = await response.Content.ReadFromJsonAsync<List<CUSessionDTO>>();

            // TODO DTO meegeven
            return View("SessionForm", emptySession);
        }

        [HttpGet("{sessionId}/edit")]
        public async Task<IActionResult> EditSession(int eventId, int sessionId)
        {
            var response = await client.GetAsync($"{eventId}/sessions/{sessionId}/editSession");
            // TODO response verwerken

            // TODO DTO meegeven
            return View("SessionForm");
        }
    }
}