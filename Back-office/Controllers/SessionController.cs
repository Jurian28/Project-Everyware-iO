using Microsoft.AspNetCore.Mvc;

namespace Back_office.Controllers
{
    [Route("{eventId}/sessions")]
    public class SessionController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public SessionController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int eventId)
        {
            var client = _httpClientFactory.CreateClient("DatabaseApi");
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
    }
}