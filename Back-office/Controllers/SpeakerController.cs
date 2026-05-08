using Microsoft.AspNetCore.Mvc;
using Back_office.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace Back_office.Controllers;

[Route("{eventId}/[controller]")]
public class SpeakerController : Controller
{
    private readonly HttpClient _client;

        /// <summary>
        /// Controller responsible for handling user authentication actions such as login, registration, and logout.
        /// </summary>
        public SpeakerController(IHttpClientFactory httpClientFactory)
        {
            _client = httpClientFactory.CreateClient("DatabaseApi");
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
            HttpResponseMessage response = await _client.GetAsync($"/speaker?eventId={eventId}");
            ApiResponse<List<SpeakerDTO>>? apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<SpeakerDTO>>>();

            return StatusCode((int)response.StatusCode, apiResponse);
        }

        /// <summary>
        /// post or put for speaker used in js on speaker/index.cshtml
        /// </summary>
        [HttpPost("data")]
        public async Task<IActionResult> SaveSpeaker([FromForm] SpeakerDTO form)
        {
            using MultipartFormDataContent content = BuildSpeakerContent(form, includeEvent: true);

            HttpResponseMessage response = await _client.PostAsync("/speaker", content);

            string result = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, result);
        }

        [HttpPut("data/{id}")]
        public async Task<IActionResult> UpdateSpeaker(int id, [FromForm] SpeakerDTO form)
        {
            using MultipartFormDataContent content = BuildSpeakerContent(form, includeEvent: false);

            HttpResponseMessage response = await _client.PutAsync($"/speaker/{id}", content);

            string result = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, result);
        }

        /// <summary>
        /// delete for speakers used in js on speaker/index.cshtml
        /// </summary>
        [HttpDelete("{idSpeaker}")]
        public async Task<IActionResult> DeleteSpeaker(int idSpeaker)
        {
            HttpResponseMessage response = await _client.DeleteAsync($"speaker/{idSpeaker}");

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

            HttpResponseMessage response = await _client.GetAsync(normalizedPath);
            if (!response.IsSuccessStatusCode)
            {
                string fallbackSvg = """
                    <svg xmlns="http://www.w3.org/2000/svg" width="40" height="40" viewBox="0 0 16 16" fill="#6c757d">
                        <path d="M8 8a3 3 0 1 0 0-6 3 3 0 0 0 0 6z"/>
                        <path fill-rule="evenodd" d="M8 16A8 8 0 1 0 8 0a8 8 0 0 0 0 16zm0-1a7 7 0 0 1-5.468-2.63c.35-.65 1.17-1.37 2.468-1.37h6c1.299 0 2.118.72 2.468 1.37A7 7 0 0 1 8 15z"/>
                    </svg>
                    """;
                return Content(fallbackSvg, "image/svg+xml");
            }

            MediaTypeHeaderValue? contentType = response.Content.Headers.ContentType;
            string mimeType = contentType?.MediaType ?? "application/octet-stream";
            Stream stream = await response.Content.ReadAsStreamAsync();

            return File(stream, mimeType);
        }

        private static MultipartFormDataContent BuildSpeakerContent(SpeakerDTO form, bool includeEvent)
        {
            MultipartFormDataContent content = new MultipartFormDataContent
            {
                { new StringContent(form.FirstName ?? string.Empty), "firstName" },
                { new StringContent(form.MiddleName ?? string.Empty), "middleName" },
                { new StringContent(form.LastName ?? string.Empty), "lastName" },
                { new StringContent(form.Description ?? string.Empty), "description" }
            };

            if (includeEvent)
            {
                content.Add(new StringContent(form.IdEvent.ToString()), "idEvent");
            }

            if (form.Image != null && form.Image.Length > 0)
            {
                Stream stream = form.Image.OpenReadStream();
                StreamContent fileContent = new StreamContent(stream);
                if (!string.IsNullOrWhiteSpace(form.Image.ContentType) &&
                    MediaTypeHeaderValue.TryParse(form.Image.ContentType, out MediaTypeHeaderValue mediaType))
                {
                    fileContent.Headers.ContentType = mediaType;
                }

                content.Add(fileContent, "ImgFile", form.Image.FileName);
            }

            return content;
        }
}
