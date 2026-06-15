using Back_office.DTOs;
using Back_office.Models.Dtos;
using Back_office.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedClassLibrary.DTOs.Auth;

namespace Back_office.Controllers;

/// <summary>
/// Controller responsible for handling Organiser management.
/// </summary>
[Route("[controller]")]
[Authorize]
public class EventOrganiserController(IHttpClientFactory httpClientFactory) : Controller
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("DatabaseApi");

    /// <summary>
    /// Displays the organisers management view.
    /// </summary>
    /// <returns>The login view.</returns>
    [HttpGet("Organisers")]
    [Authorize(Roles = "Admin")]
    public IActionResult Organisers()
    {
        return View("OrganisersManagement");
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("accept-request/{userId}")]
    public async Task<HttpResponseMessage> AcceptRequest(string userId, [FromQuery] string role = "Organiser")
    {
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"EventOrganiser/instate-role/{userId}", new { Role = role });
        return response;
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("organiser/{userId}/deny")]
    public async Task<HttpResponseMessage> DenyRequest(string userId)
    {
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"EventOrganiser/remove-organiser-request/{userId}", new { Garbage = 0 });
        return response;
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("organiser/deny/all")]
    public async Task<HttpResponseMessage> DenyAllRequests(string userId)
    {
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"EventOrganiser/remove-organiser-request", new { Garbage = 0 });
        return response;
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("organiser/{userId}/revoke")]
    public async Task<HttpResponseMessage> RevokeOrganiser(string userId)
    {
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"EventOrganiser/revoke-organiser/{userId}", new { Garbage = 0 });
        return response;
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("speaker/{userId}/revoke")]
    public async Task<HttpResponseMessage> RevokeSpeaker(string userId)
    {
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"EventOrganiser/revoke-speaker/{userId}", new { Garbage = 0 });
        return response;
    }

    /// <summary>
    /// Displays the view to request Organiser permissions.
    /// </summary>
    [HttpGet("No-Permission")]
    [Authorize]
    public async Task<IActionResult> NoPermission()
    {
        bool openRequest = false;
        HttpResponseMessage response = await _httpClient.GetAsync("EventOrganiser/has-requested-organiser-access");
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();
            openRequest = result?.Data ?? false;
        }
        return View("NoPermission", openRequest);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("data/Organisers")]
    public async Task<IActionResult> OrganisersData()
    {
        HttpResponseMessage response = await _httpClient.GetAsync($"EventOrganiser/organisers");
        ApiResponse<List<UserDTO>>? apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<UserDTO>>>();

        return StatusCode((int)response.StatusCode, apiResponse);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("data/Organisers-requests")]
    public async Task<IActionResult> OrganisersRequestData()
    {
        HttpResponseMessage response = await _httpClient.GetAsync($"EventOrganiser/organiser-requests");
        ApiResponse<List<UserDTO>>? apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<UserDTO>>>();

        return StatusCode((int)response.StatusCode, apiResponse);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("data/Speakers")]
    public async Task<IActionResult> SpeakersData()
    {
        HttpResponseMessage response = await _httpClient.GetAsync($"EventOrganiser/speakers");
        ApiResponse<List<UserDTO>>? apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<UserDTO>>>();

        return StatusCode((int)response.StatusCode, apiResponse);
    }

    /// <summary>
    /// Handles requesting of Organiser permissions
    /// </summary>
    [HttpPost("Request-Permission")]
    [Authorize]
    public async Task<HttpResponseMessage> requestPermission()
    {
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync("EventOrganiser/request-organiser-access", new { Garbage = 0 });
        return response;
    }
}
