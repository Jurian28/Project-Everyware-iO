using Microsoft.AspNetCore.Mvc;
using Back_office.DTOs;
using SharedClassLibrary.ApiResponse;

namespace Back_office.Controllers
{
    [Route("{eventId}/[controller]")]
    public class RoomController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public RoomController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public IActionResult Index(int eventId)
        {
            return View(eventId);
        }


        [HttpGet("data")]
        public async Task<IActionResult> GetRoomsForEvent(int eventId)
        {
            var client = _httpClientFactory.CreateClient("DatabaseApi");
            var response = await client.GetAsync($"/room?eventId={eventId}");
            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<RoomDTO>>>();

            return StatusCode((int)response.StatusCode, apiResponse);
        }


        [HttpPost("data")]
        public async Task<IActionResult> SaveRoom([FromBody] RoomDTO room)
        {
            var client = _httpClientFactory.CreateClient("DatabaseApi");

            HttpResponseMessage response;

            if (room.IdRoom == null || room.IdRoom == 0)
            {
                response = await client.PostAsJsonAsync("room", room);
            }
            else
            {
                response = await client.PutAsJsonAsync($"room/{room.IdRoom}", room);
            }

            var content = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, content);
        }

        [HttpDelete("{idRoom}")]
        public async Task<IActionResult> DeleteRoom(int idRoom)
        {
            var client = _httpClientFactory.CreateClient("DatabaseApi");

            HttpResponseMessage response = await client.DeleteAsync("room/"+ idRoom);

            var content = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, content);
        }
    }
}
