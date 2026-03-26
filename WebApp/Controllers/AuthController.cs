using Microsoft.AspNetCore.Mvc;
using WebApp.Models.ViewModels;

namespace WebApp.Controllers;

public class AuthResponse
{
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
}

public class AuthController(IHttpClientFactory httpClientFactory) : Controller
{
    private static readonly string AUTH_API_BASE_URL = "http://databaseapi:5000/api/auth";

    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("ApiClient");

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [ValidateAntiForgeryToken]
    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            return View(viewModel);
        }

        string loginUrl = $"{AUTH_API_BASE_URL}/login";
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(loginUrl, new
        {
            viewModel.Email,
            viewModel.Password
        });
        AuthResponse? json = await response.Content.ReadFromJsonAsync<AuthResponse>();

        if (response.IsSuccessStatusCode && json != null)
        {
            SetTokenCookies(json);

            return RedirectToAction("Index", "Home");
        }
        else
        {
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");

            return View(viewModel);
        }
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [ValidateAntiForgeryToken]
    [HttpPost]
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

        string registerUrl = $"{AUTH_API_BASE_URL}/register";
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(registerUrl, new
        {
            viewModel.Email,
            viewModel.Password
        });
        AuthResponse? json = await response.Content.ReadFromJsonAsync<AuthResponse>();

        if (response.IsSuccessStatusCode && json != null)
        {
            SetTokenCookies(json);

            return RedirectToAction("Index", "Home");
        }
        else
        {
            ModelState.AddModelError(string.Empty, await response.Content.ReadAsStringAsync());

            return View(viewModel);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        Response.Cookies.Delete("AccessToken");
        Response.Cookies.Delete("RefreshToken");

        string logoutUrl = $"{AUTH_API_BASE_URL}/logout";
        await _httpClient.PostAsync(logoutUrl, null);

        return RedirectToAction("Index", "Home");
    }

    private void SetTokenCookies(AuthResponse authResponse)
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
