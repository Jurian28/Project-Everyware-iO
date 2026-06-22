using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Back_office.DTOs;
using Back_office.Models.ViewModels;

namespace Back_office.Controllers;

/// <summary>
/// Controller for creating and accepting event invitations.
/// </summary>
[Authorize]
[Route("invites")]
public class EventInviteController(IHttpClientFactory httpClientFactory) : Controller
{
    private static string _apiBaseUrl = "http://databaseapi:5000";

    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("ApiClient");

    /// <summary>
    /// Accepts an event invitation by forwarding the token to the API and redirecting with a toast message.
    /// </summary>
    /// <param name="token">The short invite token identifying the invitation.</param>
    /// <returns>A redirect to the Events index with a success or error toast message.</returns>
    [HttpGet("accept/{token}")]
    public async Task<IActionResult> Accept(string token)
    {
        HttpResponseMessage response = await _httpClient.PostAsync($"{_apiBaseUrl}/api/invites/accept/{token}", null);
        string body = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            TempData["ToastMessage"] = "You have successfully joined the event!";
            TempData["ToastType"] = "success";
        }
        else
        {
            System.Text.Json.JsonElement json = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(body);
            string error = json.TryGetProperty("error", out System.Text.Json.JsonElement e) ? e.GetString() ?? "Failed to accept invite." : "Failed to accept invite.";
            TempData["ToastMessage"] = error;
            TempData["ToastType"] = "danger";
        }

        return RedirectToAction("Index", "Events");
    }

    /// <summary>
    /// Creates a new event invite and returns the generated invite token.
    /// </summary>
    /// <param name="createInviteViewModel">The details required to create an invite, including the target event ID and expiration date.</param>
    /// <returns>An action result containing the generated token if successful, or an error payload.</returns>
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromForm] CreateInviteViewModel createInviteViewModel)
    {
        if (createInviteViewModel.EventId == 0 || createInviteViewModel.Expires.Date < DateTime.UtcNow.Date)
        {
            return BadRequest(new
            {
                Success = false,
                Data = (object?)null,
                Error = $"Invalid event ID or expiration date. Please provide valid data. Event id: {createInviteViewModel.EventId}, Expiration date: {createInviteViewModel.Expires}"
            });
        }

        HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}/api/invites/create", createInviteViewModel);
        CreateInviteDto? json = await response.Content.ReadFromJsonAsync<CreateInviteDto>();

        if (!response.IsSuccessStatusCode)
        {
            return StatusCode((int)response.StatusCode, new
            {
                Success = false,
                Data = (object?)null,
                Error = json?.Error ?? "Failed to create invite. Please try again later."
            });
        }

        if (json?.Data?.Token == null)
        {
            return StatusCode(500, new
            {
                Success = false,
                Data = (object?)null,
                Error = "Failed to create invite. Please try again later."
            });
        }

        return StatusCode(201, new
        {
            Success = true,
            Data = new
            {
                Token = json.Data.Token
            },
            Error = (string?)null
        });
    }
}
