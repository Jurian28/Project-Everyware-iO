using Back_office.DTO;
using Microsoft.AspNetCore.Mvc;
using SharedClassLibrary.ApiResponse;

namespace Back_office.Controllers
{
    [Route("{eventId}/[controller]")]
    public class TagController : Controller
    {
        private readonly HttpClient client;

        public TagController(IHttpClientFactory httpClientFactory)
        {
            client = httpClientFactory.CreateClient("DatabaseApi");
        }

        public IActionResult Index(int eventId)
        {
            ViewData["EventId"] = eventId;
            return View(eventId);
        }

        // GET DATA
        [HttpGet("data")]
        public async Task<IActionResult> GetTagsForEvent(int eventId)
        {
            var response = await client.GetAsync($"/tag?eventId={eventId}");

            var apiResponse =
                await response.Content.ReadFromJsonAsync<ApiResponse<List<TagDto>>>();

            return StatusCode((int)response.StatusCode, apiResponse);
        }

        // CREATE / UPDATE (now based on IdTag)
        [HttpPost("data")]
        public async Task<IActionResult> SaveTag([FromBody] TagDto tag) {
            HttpResponseMessage response;

            // CREATE
            if (tag.IdTag == 0) {
                var dto = new TagDto {
                    IdEvent = tag.IdEvent,
                    Title = tag.Title,
                    ColorHex = tag.ColorHex
                };

                response = await client.PostAsJsonAsync("/tag", dto);
            }
            // UPDATE
            else {
                var dto = new TagDto {
                    Title = tag.Title,
                    ColorHex = tag.ColorHex
                };

                response = await client.PutAsJsonAsync($"/tag/{tag.IdTag}", dto);
            }

            var content = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, content);
        }

        // DELETE
        [HttpDelete("{idTag}")]
        public async Task<IActionResult> DeleteTag(int idTag)
        {
            var response = await client.DeleteAsync($"/tag/{idTag}");

            var content = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, content);
        }
    }
}
