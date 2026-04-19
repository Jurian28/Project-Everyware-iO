using Back_office.DTOs;
using Back_office.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SharedClassLibrary.DTOs.Events;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Back_office.Controllers
{
    [Route("[controller]")]
    public class EventsController : Controller
    {
        private readonly HttpClient _httpClient;

        private readonly int _PageSize = 10;

        /// <summary>
        /// Controller for handling events
        /// </summary>
        public EventsController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("DatabaseApi");
        }

        /// <summary>
        /// Events listing with pagination and search functionality
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index(string? search, int page = 1)
        {
            try
            {
                string url = $"/event?page={page}&pageSize={_PageSize}";

                if (!string.IsNullOrEmpty(search))
                    url += $"&title={search}";
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                Debug.Write(response);
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error in getting events: {response.StatusCode} | {response.ReasonPhrase}");
                    return RedirectToAction("Index", "Home");
                }
                ApiResponse<EventListDto>? json = await response.Content.ReadFromJsonAsync<ApiResponse<EventListDto>>();

                if (json == null)
                {
                    Console.WriteLine("Error in getting events");
                    return View("Index", "Home");
                }
                EventListDto data = json.Data;

                List <EventDTO> events = data?.Events ?? new List<EventDTO>();
                int pages = data?.TotalPages ?? 1;
                if(page >= pages)
                {
                    page = pages-1;
                }
                else if(page < 1)
                {
                    page = 1;
                }

                ViewData["CurrentSearch"] = search;
                ViewData["CurrentPage"] = page;
                ViewData["TotalPages"] = pages;
                return View("Index", events);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in getting event: {ex.Message}");
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Event creation page
        /// </summary>
        [HttpGet]
        [Route("create")]
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Event edition page, with pre-filled data of the selected event
        /// </summary>
        [HttpGet]
        [Route("{id}/edit")]
        public async Task<IActionResult> Edit(int id)
        {
            string url = $"/event/{id}";

            HttpResponseMessage response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "Events");
            }
            ApiResponse<EventUpdateDTO>? json = await response.Content.ReadFromJsonAsync<ApiResponse<EventUpdateDTO>>();

            if(json == null || json.Data == null)
            {
                Console.WriteLine($"Error in getting event, ID: {id}");
                return RedirectToAction("Index", "Events");
            }
                
            return View(json?.Data);
        }

        /// <summary>
        /// Get image from the API and return it as a file result, used for displaying event logos
        /// </summary>
        [HttpGet("images/{fileName}")]
        public async Task<IActionResult> GetImage(string fileName)
        {
            Console.WriteLine($"loading image");
            var response = await _httpClient.GetAsync($"/event/images/{fileName}");

            if (!response.IsSuccessStatusCode)
                return NotFound();

            byte[] imageBytes = await response.Content.ReadAsByteArrayAsync();
            string mimeType = response.Content.Headers.ContentType?.MediaType ?? "image/jpeg";
            return File(imageBytes, mimeType);
        }

        [HttpPost]
        [Route("{id}/publish")]
        public async Task<IActionResult> Publish(int id)
        {
            try
            {
                string url = $"/event/{id}/publish";
                 
                HttpResponseMessage response = await _httpClient.PostAsync(url, null);

                return StatusCode((int)response.StatusCode);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        [Route("{id}/unpublish")]
        public async Task<IActionResult> Unpublish(int id)
        {
            try
            {
                string url = $"/event/{id}/unpublish";

                HttpResponseMessage response = await _httpClient.PostAsync(url, null);
                return StatusCode((int)response.StatusCode);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Store method for creating a new event, with form data validation
        /// </summary>
        [HttpPost("store")]
        public async Task<IActionResult> Store(EventCreateDto eventDTO) 
        {
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"Validation error: {error.ErrorMessage}");
                }
                return View("Create", eventDTO);
            }

            try
            {
                string url = $"/event";
                using var content = new MultipartFormDataContent();

                content.Add(new StringContent(eventDTO.Title ?? ""), "Title");
                content.Add(new StringContent(eventDTO.Location ?? ""), "Location");
                content.Add(new StringContent(eventDTO.Description ?? ""), "Description");
                content.Add(new StringContent(eventDTO.MainColorHex ?? ""), "MainColorHex");
                content.Add(new StringContent(eventDTO.AccentColorHex ?? ""), "AccentColorHex");

                content.Add(new StringContent(eventDTO.StartDate.ToString("o")), "StartDate");
                content.Add(new StringContent(eventDTO.EndDate.ToString("o")), "EndDate");

                IFormFile? logoFile = eventDTO.LogoFile;
                if (logoFile != null)
                {
                    var fileStream = logoFile.OpenReadStream();
                    var fileContent = new StreamContent(fileStream);
                    content.Add(fileContent, "LogoFile", logoFile.FileName);
                }

                HttpResponseMessage response = await _httpClient.PostAsync(url, content);

                if (!response.IsSuccessStatusCode)
                {
                    ModelState.AddModelError(string.Empty, "Error in Store method...");
                    Console.WriteLine($"Error in Store method: {response.StatusCode} | {response.ReasonPhrase}");

                    TempData["ToastMessage"] = "Error in creating event!";
                    TempData["ToastType"] = "danger";

                    return View("Create", eventDTO);
                }
                TempData["ToastMessage"] = "Event created successfully!";
                TempData["ToastType"] = "success";

                return RedirectToAction("Index");
                
            }
            catch (Exception ex)
            {
                return View("Create", eventDTO);
            }
        }

        /// <summary>
        /// Update method for updating an existing event, with form data validation
        /// </summary>
        [HttpPost("update")]
        public async Task<IActionResult> Update(EventUpdateDTO eventDTO)
        {
            if (!ModelState.IsValid)
            {
                Console.WriteLine($"Validation error: {ModelState.Values}");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"Validation error: {error.ErrorMessage}");
                }
                return View("Index", "Home");
            }

            try
            {
                string url = $"/event/{eventDTO.IdEvent}";
                using var content = new MultipartFormDataContent();

                content.Add(new StringContent(eventDTO.Title ?? ""), "Title");
                content.Add(new StringContent(eventDTO.Location ?? ""), "Location");
                content.Add(new StringContent(eventDTO.Description ?? ""), "Description");
                content.Add(new StringContent(eventDTO.MainColorHex ?? ""), "MainColorHex");
                content.Add(new StringContent(eventDTO.AccentColorHex ?? ""), "AccentColorHex");

                content.Add(new StringContent(eventDTO.StartDate.ToString("o")), "StartDate");
                content.Add(new StringContent(eventDTO.EndDate.ToString("o")), "EndDate");
                IFormFile? logoFile = eventDTO.LogoFile;
                if (logoFile != null)
                {
                    var fileStream = logoFile.OpenReadStream();
                    var fileContent = new StreamContent(fileStream);
                    content.Add(fileContent, "LogoFile", logoFile.FileName);
                }

                HttpResponseMessage response = await _httpClient.PutAsync(url, content);
                ApiResponse<EventUpdateDTO>? responseObject = await response.Content.ReadFromJsonAsync<ApiResponse<EventUpdateDTO>>();

                if (!response.IsSuccessStatusCode)
                {
                    ModelState.AddModelError(string.Empty, "Error in Update method...");
                    Console.WriteLine($"Error in Update method: {response.StatusCode} | {responseObject?.Error}");

                    TempData["ToastMessage"] = "Error in updating event!";
                    TempData["ToastType"] = "danger";

                    return View("Edit", eventDTO);
                }
                TempData["ToastMessage"] = "Event updated successfully!";
                TempData["ToastType"] = "success";

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View("Index", "Home");
            }
        }
    }
}
