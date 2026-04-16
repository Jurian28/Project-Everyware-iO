using Back_office.Models;
using Back_office.Models.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using System.Reflection;

namespace Back_office.Controllers
{
    [Route("events")]
    public class EventsController(IHttpClientFactory httpClientFactory) : Controller
    {
        private static readonly string API_BASE_URL = Environment.GetEnvironmentVariable("API_BASE_URL") ?? "http://databaseapi:5000";
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient("ApiClient");

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Index()
        {
            try
            {
                string url = $"{API_BASE_URL}/api/event";

                HttpResponseMessage response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    ApiResponse<List<Event>>? json = await response.Content.ReadFromJsonAsync<ApiResponse<List<Event>>>();

                    if (json != null)
                    {
                        return View("Index", json.Data);
                    } 
                    else
                    {
                        Console.WriteLine("Error in getting events");
                        return View("Index", new List<Event>());
                    }
                }
                else
                {
                    Console.WriteLine($"Error in getting events: {response.StatusCode} | {response.ReasonPhrase}");
                    return View("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in getting event: {ex.Message}");
                return View("Index", "Home");
            }
        }

        [HttpGet]
        [Route("create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpGet]
        [Route("{id}/edit")]
        public async Task<IActionResult> Edit(int id)
        {
            string url = $"{API_BASE_URL}/api/event/{id}";

            HttpResponseMessage response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                ApiResponse<Event>? json = await response.Content.ReadFromJsonAsync<ApiResponse<Event>>();

                if(json == null || json.Data == null)
                {
                    Console.WriteLine($"Error in getting event, ID: {id}");
                    return RedirectToAction("Index", "Events");
                }

                return View(json?.Data);
            }
            else
            {
                return RedirectToAction("Index", "Events");
            }
        }

        [HttpPost("store")]
        public async Task<IActionResult> Store(Event eventModel, IFormFile? logoFile) 
        {
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"Validation error: {error.ErrorMessage}");
                }
                return View("Create", eventModel);
            }

            try
            {
                string url = $"{API_BASE_URL}/api/event";
                using var content = new MultipartFormDataContent();

                content.Add(new StringContent(eventModel.Title ?? ""), "Title");
                content.Add(new StringContent(eventModel.Location ?? ""), "Location");
                content.Add(new StringContent(eventModel.Description ?? ""), "Description");
                content.Add(new StringContent(eventModel.MainColorHex ?? ""), "MainColorHex");
                content.Add(new StringContent(eventModel.AccentColorHex ?? ""), "AccentColorHex");

                content.Add(new StringContent(eventModel.StartDate.ToString("o")), "StartDate");
                content.Add(new StringContent(eventModel.EndDate.ToString("o")), "EndDate");

                if (logoFile != null)
                {
                    var fileStream = logoFile.OpenReadStream();
                    var fileContent = new StreamContent(fileStream);
                    content.Add(fileContent, "LogoFile", logoFile.FileName);
                }

                HttpResponseMessage response = await _httpClient.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["ToastMessage"] = "Event created successfully!";
                    TempData["ToastType"] = "success";

                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error in Store method...");
                    Console.WriteLine($"Error in Store method: {response.StatusCode} | {response.ReasonPhrase}");

                    TempData["ToastMessage"] = "Error in creating event!";
                    TempData["ToastType"] = "danger";

                    return View("Create", eventModel);
                }
            }
            catch (Exception ex)
            {
                return View("Create", eventModel);
            }
        }

        [HttpPost("update")]
        public async Task<IActionResult> Update(Event eventModel, IFormFile? logoFile)
        {
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"Validation error: {error.ErrorMessage}");
                }
                return View("Edit", eventModel);
            }

            try
            {
                string url = $"{API_BASE_URL}/api/event/{eventModel.IdEvent}";
                using var content = new MultipartFormDataContent();

                content.Add(new StringContent(eventModel.Title ?? ""), "Title");
                content.Add(new StringContent(eventModel.Location ?? ""), "Location");
                content.Add(new StringContent(eventModel.Description ?? ""), "Description");
                content.Add(new StringContent(eventModel.MainColorHex ?? ""), "MainColorHex");
                content.Add(new StringContent(eventModel.AccentColorHex ?? ""), "AccentColorHex");

                content.Add(new StringContent(eventModel.StartDate.ToString("o")), "StartDate");
                content.Add(new StringContent(eventModel.EndDate.ToString("o")), "EndDate");

                if (logoFile != null)
                {
                    var fileStream = logoFile.OpenReadStream();
                    var fileContent = new StreamContent(fileStream);
                    content.Add(fileContent, "LogoFile", logoFile.FileName);
                }

                HttpResponseMessage response = await _httpClient.PutAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["ToastMessage"] = "Event updated successfully!";
                    TempData["ToastType"] = "success";

                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error in Update method...");
                    Console.WriteLine($"Error in Store method: {response.StatusCode} | {response.ReasonPhrase}");

                    TempData["ToastMessage"] = "Error in updating event!";
                    TempData["ToastType"] = "danger";

                    return View("Edit", eventModel);
                }
            }
            catch (Exception ex)
            {
                return View("Edit", eventModel);
            }
        }
    }
}
