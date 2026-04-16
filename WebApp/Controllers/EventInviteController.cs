using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models.Dtos;
using WebApp.Models.ViewModels;

namespace WebApp.Controllers;

[Authorize]
[Route("invites")]
public class EventInviteController(IHttpClientFactory httpClientFactory) : Controller
{
    private static readonly string API_BASE_URL = "http://databaseapi:5000/api/invites";

    private static readonly string INVALID_INVITE_MESSAGE = "This invite is not valid.";
    private static readonly string FAILED_ACCEPT_MESSAGE = "Something went wrong while trying to accept the invite. Please try again later.";

    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("ApiClient");

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromForm] CreateInviteViewModel createInviteViewModel)
    {
        if (createInviteViewModel.EventId == 0 || createInviteViewModel.Expires < DateTime.UtcNow)
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

        if (!response.IsSuccessStatusCode || json?.Data == null)
        {
            return StatusCode(500, new
            {
                Success = false,
                Data = (object?)null,
                Error = json?.Error ?? "Failed to create invite. Please try again later."
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
