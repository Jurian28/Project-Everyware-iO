using Microsoft.AspNetCore.Mvc;
using Back_office.DTOs;
using SharedClassLibrary.ApiResponse;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http.Headers;

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
        public async Task<IActionResult> SaveSpeaker(IFormCollection form)
        {
            using var content = new MultipartFormDataContent();

            content.Add(new StringContent(form["firstName"]), "firstName");
            content.Add(new StringContent(form["middleName"]), "middleName");
            content.Add(new StringContent(form["lastName"]), "lastName");
            content.Add(new StringContent(form["description"]), "description");
            content.Add(new StringContent(form["idEvent"]), "idEvent");

            if (form.Files.Count > 0)
            {
                var file = form.Files[0];

                var stream = file.OpenReadStream();
                var fileContent = new StreamContent(stream);
                if (!string.IsNullOrWhiteSpace(file.ContentType) &&
                    MediaTypeHeaderValue.TryParse(file.ContentType, out var mediaType))
                {
                    fileContent.Headers.ContentType = mediaType;
                }

                content.Add(fileContent, "image", file.FileName);
            }

            HttpResponseMessage response = await client.PostAsync("/speaker", content);

            string result = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, result);
        }

        [HttpPut("data/{id}")]
        public async Task<IActionResult> UpdateSpeaker(int id, IFormCollection form)
        {
            using var content = new MultipartFormDataContent();

            content.Add(new StringContent(form["firstName"]), "firstName");
            content.Add(new StringContent(form["middleName"]), "middleName");
            content.Add(new StringContent(form["lastName"]), "lastName");
            content.Add(new StringContent(form["description"]), "description");

            if (form.Files.Count > 0)
            {
                var file = form.Files[0];

                var stream = file.OpenReadStream();
                var fileContent = new StreamContent(stream);
                if (!string.IsNullOrWhiteSpace(file.ContentType) &&
                    MediaTypeHeaderValue.TryParse(file.ContentType, out var mediaType))
                {
                    fileContent.Headers.ContentType = mediaType;
                }

                content.Add(fileContent, "image", file.FileName);
            }

            HttpResponseMessage response = await client.PutAsync($"/speaker/{id}", content);

            string result = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, result);
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

        /// <summary>
        /// Proxies speaker images from DatabaseApi so the browser only calls Back-office.
        /// </summary>
        [HttpGet("image")]
        public async Task<IActionResult> GetSpeakerImage(int eventId, [FromQuery] string? imagePath)
        {
            string normalizedPath = string.IsNullOrWhiteSpace(imagePath)
                ? "/images/speakers/default.jpg"
                : imagePath.Trim();

            if (!normalizedPath.StartsWith('/'))
            {
                normalizedPath = "/" + normalizedPath;
            }

            if (!normalizedPath.StartsWith("/images/speakers/", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Invalid image path.");
            }

            HttpResponseMessage response = await client.GetAsync(normalizedPath);
            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            MediaTypeHeaderValue? contentType = response.Content.Headers.ContentType;
            string mimeType = contentType?.MediaType ?? "application/octet-stream";
            Stream stream = await response.Content.ReadAsStreamAsync();

            return File(stream, mimeType);
        }
    }
}
