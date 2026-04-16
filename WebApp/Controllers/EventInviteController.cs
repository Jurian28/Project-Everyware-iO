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
