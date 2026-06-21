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
public class RoleManagementController(IHttpClientFactory httpClientFactory) : Controller
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
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"RoleManagement/instate-role/{userId}", new { Role = role });
        return response;
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("request/{userId}/deny")]
    public async Task<HttpResponseMessage> DenyRequest(string userId)
    {
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"RoleManagement/remove-role-request/{userId}", new { Garbage = 0 });
        return response;
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("request/deny/all")]
    public async Task<HttpResponseMessage> DenyAllRequests(string userId)
    {
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"RoleManagement/remove-role-request", new { Garbage = 0 });
        return response;
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("revoke-role/{userId}")]
    public async Task<HttpResponseMessage> RevokeRole(string userId, [FromQuery] string role)
    {
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"RoleManagement/revoke-role/{userId}", new { Role = role });
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
        HttpResponseMessage response = await _httpClient.GetAsync("RoleManagement/has-requested-role-access");
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
        HttpResponseMessage response = await _httpClient.GetAsync($"RoleManagement/organisers");
        ApiResponse<List<UserDTO>>? apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<UserDTO>>>();

        return StatusCode((int)response.StatusCode, apiResponse);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("data/role-requests")]
    public async Task<IActionResult> OrganisersRequestData()
    {
        HttpResponseMessage response = await _httpClient.GetAsync($"RoleManagement/role-requests");
        ApiResponse<List<UserDTO>>? apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<UserDTO>>>();

        return StatusCode((int)response.StatusCode, apiResponse);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("data/Speakers")]
    public async Task<IActionResult> SpeakersData()
    {
        HttpResponseMessage response = await _httpClient.GetAsync($"RoleManagement/speakers");
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
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync("RoleManagement/request-role-access", new { Garbage = 0 });
        return response;
    }
}
