using Microsoft.AspNetCore.Mvc;
using Back_office.DTOs;

namespace Back_office.Controllers
{
    [Route("[controller]")]
    public class RoomController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public RoomController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public IActionResult Index()
        {
            return View();
        }


        [HttpGet("data")]
        public async Task<IActionResult> GetRoomsForEvent([FromQuery] int eventId)
        {
            var client = _httpClientFactory.CreateClient("DatabaseApi");

            var response = await client.GetAsync($"room?eventId={eventId}");

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode);

            var rooms = await response.Content.ReadFromJsonAsync<List<RoomDTO>>();

            return Ok(rooms);
        }


        [HttpPost("data")]
        public async Task<IActionResult> SaveRoom([FromBody] RoomDTO room)
        {
            var client = _httpClientFactory.CreateClient("DatabaseApi");

            HttpResponseMessage response;

            // Decide based on IdRoom
            if (room.IdRoom == null || room.IdRoom == 0)
            {
                // CREATE
                response = await client.PostAsJsonAsync("room", room);
            }
            else
            {
                // UPDATE
                response = await client.PutAsJsonAsync($"room/{room.IdRoom}", room);
            }

            var content = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, content);
        }
    }
}
