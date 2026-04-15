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
        private static readonly string API_BASE_URL = "http://databaseapi:5000";
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
                        Console.WriteLine(json);
                        if(json.Data == null)
                        {
                            
                        }
                    } 
                    else
                    {
                        return View("Index", new List<Event>());
                    }

                    return View("Index", json.Data);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error in Store method...");

                    return View("Index", "Home");
                }
            }
            catch (Exception ex)
            {
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
        public IActionResult Edit(int id)
        {
            Event eventModel = new Event();
            eventModel.IdEvent = id;
            eventModel.Title = "Teen Titans Go";
            eventModel.Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.";
            eventModel.Location = "Paris, France";
            eventModel.StartDate = DateTime.Now;
            eventModel.EndDate = DateTime.Now.AddDays(2);
            eventModel.MainColorHex = "#FF5733";
            eventModel.AccentColorHex = "#33C1FF";
            eventModel.LogoPath = "/images/teen-titans-go-logo.png";

            return View(eventModel);
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
                    ApiResponse<Event>? json = await response.Content.ReadFromJsonAsync<ApiResponse<Event>>();

                    if (json != null)
                    {
                        Console.WriteLine(json);
                    }

                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error in Store method...");

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
                return View("Create", eventModel);
            }

            try
            {
                if(logoFile != null)
                {
                    Console.WriteLine($"Received file: {logoFile.FileName}");
                }
                Console.WriteLine($"Received event: {eventModel.Title}");
                // TODO

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View("Edit", eventModel);
            }
        }
    }
}
