using Microsoft.AspNetCore.Mvc;
using Back_office.DTOs;
using SharedClassLibrary.ApiResponse;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Back_office.Controllers
{
    [Route("{eventId}/[controller]")]
    public class SpeakerController : Controller
    {
        private readonly HttpClient client;

        /// <summary>
        /// Controller responsible for handling user authentication actions such as login, registration, and logout.
        /// </summary>
        public SpeakerController(IHttpClientFactory httpClientFactory)
        {
            client = httpClientFactory.CreateClient("DatabaseApi");
        }
        /// <summary>
        /// Speaker crud page
        /// </summary>
        public IActionResult Index(int eventId)
        {
            ViewData["EventId"] = eventId;
            return View(eventId);
        }

        /// <summary>
        /// get for speaker used in js on speaker/index.cshtml
        /// </summary>
        [HttpGet("data")]
        public async Task<IActionResult> GetSpeakersForEvent(int eventId)
        {
            HttpResponseMessage response = await client.GetAsync($"/speaker?eventId={eventId}");
            ApiResponse<List<SpeakerDTO>>? apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<SpeakerDTO>>>();

            return StatusCode((int)response.StatusCode, apiResponse);
        }

        /// <summary>
        /// post or put for speaker used in js on speaker/index.cshtml
        /// </summary>
        [HttpPost("data")]
        public async Task<IActionResult> SaveSpeaker([FromBody] SpeakerDTO speaker)
        {
            HttpResponseMessage response;

            if (speaker.IdSpeaker == null || speaker.IdSpeaker == 0)
            {
                response = await client.PostAsJsonAsync("speaker", speaker);
            }
            else
            {
                response = await client.PutAsJsonAsync($"speaker/{speaker.IdSpeaker}", speaker);
            }

            string content = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, content);
        }

        /// <summary>
        /// delete for speakers used in js on speaker/index.cshtml
        /// </summary>
        [HttpDelete("{idSpeaker}")]
        public async Task<IActionResult> DeleteSpeaker(int idSpeaker)
        {
            HttpResponseMessage response = await client.DeleteAsync("speaker/" + idSpeaker);

            string content = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, content);
        }
    }
}
