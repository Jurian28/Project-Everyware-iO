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
    private static string API_BASE_URL = "http://databaseapi:5000";

    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("ApiClient");

    /// <summary>
    /// Creates a new event invite and generates an invite link.
    /// </summary>
    /// <param name="createInviteViewModel">The details required to create an invite, including the target event ID and expiration date.</param>
    /// <returns>An action result containing the generated invite link if successful, or an error payload.</returns>
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

        HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"{API_BASE_URL}/api/invites/create", createInviteViewModel);
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

        if (json?.Data == null)
        {
            return StatusCode(500, new
            {
                Success = false,
                Data = (object?)null,
                Error = "Failed to create invite. Please try again later."
            });
        }

        string? inviteLink = Url.Action("Accept", "EventInvite", new { inviteId = json.Data.Invite }, Request.Scheme);

        if (inviteLink == null)
        {
            return StatusCode(500, new
            {
                Success = false,
                Data = (object?)null,
                Error = "Failed to generate invite link. Please try again later."
            });
        }

        return StatusCode(201, new
        {
            Success = true,
            Data = new
            {
                InviteLink = inviteLink
            },
            Error = (string?)null
        });
    }
}
