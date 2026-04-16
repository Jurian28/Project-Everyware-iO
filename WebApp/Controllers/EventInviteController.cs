using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models.Dtos;
using WebApp.Models.ViewModels;

namespace WebApp.Controllers;

/// <summary>
/// Controller for creating and accepting event invitations.
/// </summary>
[Authorize]
[Route("invites")]
public class EventInviteController(IHttpClientFactory httpClientFactory) : Controller
{
    private static readonly string API_BASE_URL = "http://databaseapi:5000/api/invites";

    private static readonly string INVALID_INVITE_MESSAGE = "This invite is not valid.";
    private static readonly string FAILED_ACCEPT_MESSAGE = "Something went wrong while trying to accept the invite. Please try again later.";

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

        HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"{API_BASE_URL}/create", createInviteViewModel);
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

    /// <summary>
    /// Attempts to accept an event invitation using the specified ID.
    /// </summary>
    /// <param name="inviteId">The unique identifier of the invite to accept.</param>
    /// <returns>A view models displaying the result and success state of the invite acceptance.</returns>
    [HttpGet("accept/{inviteId}")]
    public async Task<IActionResult> Accept(int inviteId)
    {
        if (inviteId == 0)
        {
            return View(new AcceptInviteViewModel
            {
                Success = false,
                Message = INVALID_INVITE_MESSAGE
            });
        }

        string acceptUrl = $"{API_BASE_URL}/accept/{inviteId}";
        HttpResponseMessage response = await _httpClient.PostAsync(acceptUrl, null);
        AcceptInviteDto? json = await response.Content.ReadFromJsonAsync<AcceptInviteDto>();

        if (json == null)
        {
            return View(new AcceptInviteViewModel
            {
                Success = false,
                Message = FAILED_ACCEPT_MESSAGE
            });
        }

        if (!response.IsSuccessStatusCode)
        {
            return View(new AcceptInviteViewModel
            {
                Success = false,
                Message = json?.Error ?? FAILED_ACCEPT_MESSAGE
            });
        }

        return View(new AcceptInviteViewModel
        {
            Success = json.Success,
            Message = string.Empty
        });
    }
}
