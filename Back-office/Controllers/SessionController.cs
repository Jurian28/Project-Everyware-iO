using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedClassLibrary.DTOs.Sessions;
using SharedClassLibrary.DTOs.Tags;
using System.Text.Json;

namespace Back_office.Controllers
{
    [Route("{eventId}/[controller]")]
    [Authorize(Roles = "Organiser, Admin")]
    public class SessionController : Controller
    {

        private readonly HttpClient client;

        public SessionController(IHttpClientFactory httpClientFactory)
        {
            client = httpClientFactory.CreateClient("DatabaseApi");
        }

        [HttpGet]
        public async Task<IActionResult> Index(int eventId)
        {
            ViewData["EventId"] = eventId;
            HttpResponseMessage response = await client.GetAsync($"{eventId}/sessions");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Er is iets misgegaan bij het ophalen van de sessies.";
                return RedirectToAction("Index", "Home");
            }

            List<SessionDTO> sessions = (await response.Content.ReadFromJsonAsync<ApiResponse<List<SessionDTO>>>()).Data ?? new List<SessionDTO>();

            SessionListDto dto = new SessionListDto
            {
                EventId = eventId,
                Sessions = sessions
            };
            return View(dto);
        }

        [HttpGet("add")]
        public async Task<IActionResult> AddSession(int eventId, string? error = null)
        {
            ViewData["EventId"] = eventId;
            if (error != null)
            {
                TempData["ToastMessage"] = error;
                TempData["ToastType"] = "error";
            }
            HttpResponseMessage response = await client.GetAsync($"{eventId}/sessions/getAdd");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Er is iets misgegaan bij het ophalen van de bruikbare ruimtes, tags of sprekers.";
                return RedirectToAction("Index");
            }

            CUSessionDTO emptySession = (await response.Content.ReadFromJsonAsync<ApiResponse<CUSessionDTO>>()).Data ?? new CUSessionDTO();

            return View("SessionForm", emptySession);
        }

        [HttpGet("{sessionId}/edit")]
        public async Task<IActionResult> EditSession(int eventId, int sessionId)
        {
            ViewData["EventId"] = eventId;
            HttpResponseMessage response = await client.GetAsync($"{eventId}/sessions/{sessionId}/edit");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Er is iets misgegaan bij het ophalen van de sessie.";
                return RedirectToAction("Index");
            }

            CUSessionDTO session = (await response.Content.ReadFromJsonAsync<ApiResponse<CUSessionDTO>>()).Data ?? new CUSessionDTO();
            return View("SessionForm", session);
        }

        [HttpPost("save")]
        public async Task<IActionResult> HandleSubmit(int eventId, SessionDTO session, List<int> selectedTagIds)
        {
            session.Tags = selectedTagIds
                .Select(t => new TagResponseDTO { IdTag = t, IdEvent = eventId })
                .ToList()
                ?? new List<TagResponseDTO>();

            HttpResponseMessage response = await client.PostAsJsonAsync($"{eventId}/sessions/save", session);

            if (!response.IsSuccessStatusCode)
            {
                ViewData["EventId"] = eventId;
                string error = $"Kon de sessie niet opslaan: {response.ReasonPhrase}";

                return RedirectToAction("AddSession", new { eventId, error });
            }

            return RedirectToAction("Index", new { eventId = eventId });
        }

        [HttpPost("{sessionId}/delete")]
        public async Task<IActionResult> DeleteSession(int eventId, int sessionId)
        {
            HttpResponseMessage response = await client.DeleteAsync($"{eventId}/sessions/{sessionId}/delete");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Er is iets mis gegaan bij het verwijderen van de sessie.";
                return RedirectToAction("Index", new { eventId = eventId });
            }

            return RedirectToAction("Index", new { eventId = eventId });
        }

        /// <summary>
        /// Get total,open and waitinglistspots for all sessions of event or 1 session if sessionId is given
        /// </summary>
        [HttpGet("GetSpotsData/{sessionId?}")]
        public async Task<List<SessionSpotsDTO>?> GetSpotsData(int eventId, int? sessionId)
        {
            HttpResponseMessage response = await client.GetAsync(sessionId == null
                ? $"{eventId}/Sessions/GetSpotsData"
                : $"{eventId}/Sessions/GetSpotsData/{sessionId}");
            List<SessionSpotsDTO>? sessionSpotsDTOs = new();
            if (response.IsSuccessStatusCode)
            {
                sessionSpotsDTOs = (await response.Content.ReadFromJsonAsync<ApiResponse<List<SessionSpotsDTO>>>()).Data;
            }
            else
            {
                TempData["Error"] = "Error while getting Session Data";
                sessionSpotsDTOs = null;
            }
            return sessionSpotsDTOs;
        }
    }
}