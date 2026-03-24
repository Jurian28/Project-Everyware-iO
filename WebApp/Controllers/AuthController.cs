using Microsoft.AspNetCore.Mvc;
using WebApp.Models.ViewModels;

namespace WebApp.Controllers
{
    public class AuthController : Controller
    {
        private static readonly string AUTH_API_BASE_URL = "http://databaseapi:5000/api/auth";

        private readonly HttpClient _httpClient = new();

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel viewModel)
        {
            return View(viewModel);
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

            Console.WriteLine($"Response: {response.StatusCode} {await response.Content.ReadAsStringAsync()}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View(viewModel);
            }
        }
    }
}
