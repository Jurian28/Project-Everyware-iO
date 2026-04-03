using Microsoft.AspNetCore.Mvc;
using Back_office.DTOs;
using SharedClassLibrary.ApiResponse;

namespace Back_office.Controllers
{
    [Route("{eventId}/[controller]")]
    public class RoomController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        /// <summary>
        /// Controller responsible for handling user authentication actions such as login, registration, and logout.
        /// </summary>
        public RoomController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        /// <summary>
        /// room crud page
        /// </summary>
        public IActionResult Index(int eventId)
        {
            ViewData["EventId"] = eventId;
            return View(eventId);
        }

        /// <summary>
        /// get for rooms used in js on room/index.cshtml
        /// </summary>
        [HttpGet("data")]
        public async Task<IActionResult> GetRoomsForEvent(int eventId)
        {
            var client = _httpClientFactory.CreateClient("DatabaseApi");
            var response = await client.GetAsync($"/room?eventId={eventId}");
            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<RoomDTO>>>();

            return StatusCode((int)response.StatusCode, apiResponse);
        }

        /// <summary>
        /// post or put for rooms used in js on room/index.cshtml
        /// </summary>
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

        /// <summary>
        /// delete for rooms used in js on room/index.cshtml
        /// </summary>
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
