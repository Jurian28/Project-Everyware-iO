using Back_office.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Back_office.Controllers
{
    [Route("{eventId}/[controller]")]
    [Authorize(Roles = "Organiser, Admin")]
    public class RoomController : Controller
    {
        private readonly HttpClient client;

        /// <summary>
        /// Controller responsible for managing Rooms
        /// </summary>
        public RoomController(IHttpClientFactory httpClientFactory)
        {
            client = httpClientFactory.CreateClient("DatabaseApi");
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
            HttpResponseMessage response = await client.GetAsync($"/room?eventId={eventId}");
            ApiResponse<List<RoomDTO>>? apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<RoomDTO>>>();

            return StatusCode((int)response.StatusCode, apiResponse);
        }

        /// <summary>
        /// post or put for rooms used in js on room/index.cshtml
        /// </summary>
        [HttpPost("data")]
        public async Task<IActionResult> SaveRoom([FromBody] RoomDTO room)
        {
            HttpResponseMessage response;

            if (room.IdRoom == null || room.IdRoom == 0)
            {
                response = await client.PostAsJsonAsync("room", room);
            }
            else
            {
                response = await client.PutAsJsonAsync($"room/{room.IdRoom}", room);
            }

            string content = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, content);
        }

        /// <summary>
        /// delete for rooms used in js on room/index.cshtml
        /// </summary>
        [HttpDelete("{idRoom}")]
        public async Task<IActionResult> DeleteRoom(int idRoom)
        {
            HttpResponseMessage response = await client.DeleteAsync("room/" + idRoom);

            string content = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, content);
        }
    }
}
