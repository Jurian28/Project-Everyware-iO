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
            HttpResponseMessage response = await client.GetAsync($"/tag?eventId={eventId}");

            ApiResponse<List<TagDto>>? apiResponse =
                await response.Content.ReadFromJsonAsync<ApiResponse<List<TagDto>>>();

            return StatusCode((int)response.StatusCode, apiResponse);
        }

        // CREATE / UPDATE
        [HttpPost("data")]
        public async Task<IActionResult> SaveTag([FromBody] TagDto tag)
        {
            HttpResponseMessage response;

            Console.WriteLine("===========================");
            Console.WriteLine(tag.Title);
            Console.WriteLine(tag.OldTitle);
            Console.WriteLine(tag.ColorHex);
            Console.WriteLine("===========================");

            // CREATE
            if (string.IsNullOrEmpty(tag.OldTitle))
            {
                response = await client.PostAsJsonAsync("tag", tag);
            }
            // UPDATE (composite key)
            else
            {
                response = await client.PutAsJsonAsync(
                    $"tag/{tag.IdEvent}/{tag.OldTitle}",
                    tag
                );
            }

            var content = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, content);
        }

        // DELETE
        [HttpDelete("{idEvent}/{title}")]
        public async Task<IActionResult> DeleteTag(int idEvent, string title)
        {
            var response = await client.DeleteAsync(
                $"tag/{idEvent}/{title}"
            );

            var content = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, content);
        }
    }
}
