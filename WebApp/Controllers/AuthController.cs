using Microsoft.AspNetCore.Mvc;
using WebApp.Models.Dtos;
using WebApp.Models.ViewModels;

namespace WebApp.Controllers;

/// <summary>
/// Controller responsible for handling user authentication actions such as login, registration, and logout.
/// </summary>
/// <param name="httpClientFactory">The factory used to create instances of <see cref="HttpClient"/>.</param>
public class AuthController(IHttpClientFactory httpClientFactory) : Controller
{
    private static readonly string AUTH_API_BASE_URL = GetApiBaseUrl();

    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("ApiClient");

    /// <summary>
    /// Displays the login view.
    /// </summary>
    /// <returns>The login view.</returns>
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    /// <summary>
    /// Handles the submission of the login form and authenticates the user.
    /// </summary>
    /// <param name="viewModel">The view model containing the user's login credentials.</param>
    /// <returns>A redirect to the home page on success, or the login view with validation errors on failure.</returns>
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
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");

            return View(viewModel);
        }
    }

    /// <summary>
    /// Displays the registration view.
    /// </summary>
    /// <returns>The registration view.</returns>
    [HttpGet]
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
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        Response.Cookies.Delete("AccessToken");
        Response.Cookies.Delete("RefreshToken");

        string logoutUrl = $"{AUTH_API_BASE_URL}/logout";
        await _httpClient.PostAsync(logoutUrl, null);

        return RedirectToAction("Index", "Home");
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

    /// <summary>
    /// Retrieves the base URL for the API based on the current execution environment.
    /// </summary>
    /// <returns>The constructed API base URL string.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the <c>APP_URL</c> or <c>DATABASE_API_PORT</c> environment variables are not set in a non-Docker environment.</exception>
    private static string GetApiBaseUrl()
    {
        if (Environment.GetEnvironmentVariable("RUNNING_IN_DOCKER") == "true")
        {
            return "http://databaseapi:5000";
        }
        else
        {
            string apiBaseUrl = Environment.GetEnvironmentVariable("APP_URL") ?? throw new InvalidOperationException("APP_URL environment variable not set.");
            string databaseApiPort = Environment.GetEnvironmentVariable("DATABASE_API_PORT") ?? throw new InvalidOperationException("DATABASE_API_PORT environment variable not set.");

            return $"{apiBaseUrl}:{databaseApiPort}";
        }
    }
}
