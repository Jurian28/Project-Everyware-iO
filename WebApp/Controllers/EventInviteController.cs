using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models.Dtos;

namespace WebApp.Controllers;

[Authorize]
[Route("invites")]
public class EventInviteController(IHttpClientFactory httpClientFactory) : Controller
{
    private static readonly string API_BASE_URL = "http://databaseapi:5000/api/invites";

    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("ApiClient");

    [HttpGet("accept/{inviteId}")]
    public async Task<IActionResult> Accept(int inviteId)
    {
        if (inviteId == 0)
        {
            return View(false);
        }

        string acceptUrl = $"{API_BASE_URL}/accept/{inviteId}";
        HttpResponseMessage response = await _httpClient.PostAsync(acceptUrl, null);

        if (!response.IsSuccessStatusCode)
        {
            return View(false);
        }

        var json = await response.Content.ReadFromJsonAsync<AcceptInviteDto>();

        if (json == null)
        {
            return View(false);
        }

        return View(json.Success);
    }
}
