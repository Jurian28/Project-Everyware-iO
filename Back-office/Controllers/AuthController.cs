using Back_office.DTOs;
using Back_office.Models.Dtos;
using Back_office.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedClassLibrary.DTOs.Auth;

namespace Back_office.Controllers;

/// <summary>
/// Controller responsible for handling user authentication actions such as login, registration, and logout.
/// </summary>
/// <param name="httpClientFactory">The factory used to create instances of <see cref="HttpClient"/>.</param>
[Route("[controller]")]
[Authorize]
public class AuthController(IHttpClientFactory httpClientFactory) : Controller
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("DatabaseApi");

    /// <summary>
    /// Displays the login view.
    /// </summary>
    /// <returns>The login view.</returns>
    [HttpGet("Login")]
    [AllowAnonymous]
    public IActionResult Login()
    {
        return View("Login");
    }

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
    [HttpPost("organiser/{userId}/accept")] 
    public async Task<HttpResponseMessage> AcceptRequest(string userId)
    {
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"api/auth/instate-organiser/{userId}", new { Garbage = 0 });
        return response;
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("organiser/{userId}/deny")]
    public async Task<HttpResponseMessage> DenyRequest(string userId)
    {
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"api/auth/remove-organiser-request/{userId}", new { Garbage = 0 });
        return response;
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("organiser/deny/all")]
    public async Task<HttpResponseMessage> DenyAllRequests(string userId)
    {
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"api/auth/remove-organiser-request", new { Garbage = 0 });
        return response;
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("organiser/{userId}/revoke")]
    public async Task<HttpResponseMessage> RevokeOrganiser(string userId)
    {
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"api/auth/revoke-organiser/{userId}", new { Garbage = 0 });
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
        HttpResponseMessage response = await _httpClient.GetAsync("api/auth/has-requested-organiser-access");
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
        HttpResponseMessage response = await _httpClient.GetAsync($"api/auth/organisers");
        ApiResponse<List<UserDTO>>? apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<UserDTO>>>();

        return StatusCode((int)response.StatusCode, apiResponse);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("data/Organisers-requests")]
    public async Task<IActionResult> OrganisersRequestData()
    {
        HttpResponseMessage response = await _httpClient.GetAsync($"api/auth/organiser-requests");
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
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync("api/auth/request-organiser-access", new { Garbage = 0 });
        return response;
    }

    /// <summary>
    /// Handles the submission of the login form and authenticates the user.
    /// </summary>
    /// <param name="viewModel">The view model containing the user's login credentials.</param>
    /// <returns>A redirect to the home page on success, or the login view with validation errors on failure.</returns>
    [ValidateAntiForgeryToken]
    [AllowAnonymous]
    [HttpPost("Login")]
    public async Task<IActionResult> Login(LoginViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            return View(viewModel);
        }

        string loginUrl = $"api/auth/login";
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(loginUrl, new
        {
            viewModel.Email,
            viewModel.Password
        });

        if (response.IsSuccessStatusCode)
        {
            AuthOutputDto? json = await response.Content.ReadFromJsonAsync<AuthOutputDto>();

            if (json != null)
            {
                SetTokenCookies(json);
            }

            return RedirectToAction("Index", "Events");
        }
        else
        {
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");

            return View(viewModel);
        }
    }

    /// <summary>
    /// Displays the registration view.
    /// </summary>
    /// <returns>The registration view.</returns>
    [HttpGet("register")]
    [AllowAnonymous]
    public IActionResult Register()
    {
        return View();
    }

    /// <summary>
    /// Handles the submission of the registration form and creates a new user account.
    /// </summary>
    /// <param name="viewModel">The view model containing the user's registration details.</param>
    /// <returns>A redirect to the home page on success, or the registration view with validation errors on failure.</returns>
    [ValidateAntiForgeryToken]
    [HttpPost("Register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            return View(viewModel);
        }

        if (viewModel.Password != viewModel.ConfirmPassword)
        {
            ModelState.AddModelError("ConfirmPassword", "Passwords do not match.");

            return View(viewModel);
        }

        string registerUrl = $"api/auth/register";
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(registerUrl, new
        {
            viewModel.Email,
            viewModel.Password
        });

        if (response.IsSuccessStatusCode)
        {
            AuthOutputDto? json = await response.Content.ReadFromJsonAsync<AuthOutputDto>();

            if (json != null)
            {
                SetTokenCookies(json);
            }

            return RedirectToAction("Index", "Home");
        }
        else
        {
            ModelState.AddModelError(string.Empty, await response.Content.ReadAsStringAsync());

            return View(viewModel);
        }
    }

    /// <summary>
    /// Logs the user out by clearing the authentication cookies and notifying the authentication API.
    /// </summary>
    /// <returns>A redirect to the home page.</returns>
    [HttpPost("Logout")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        Response.Cookies.Delete("AccessToken");
        Response.Cookies.Delete("RefreshToken");

        string logoutUrl = $"api/auth/logout";
        await _httpClient.PostAsync(logoutUrl, null);

        return RedirectToAction("Login");
    }

    /// <summary>
    /// Sets the access and refresh tokens as HTTP-only, secure cookies in the response.
    /// </summary>
    /// <param name="authResponse">The Data Transfer Object containing the access and refresh tokens.</param>
    private void SetTokenCookies(AuthOutputDto authResponse)
    {
        Response.Cookies.Append("AccessToken", authResponse.AccessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.Now.AddHours(1)
        });

        Response.Cookies.Append("RefreshToken", authResponse.RefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.Now.AddDays(7)
        });
    }
}
