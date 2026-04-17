using SharedClassLibrary.DTOs.Events;
using DatabaseApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using DatabaseApi.DTOs;

namespace DatabaseApi.Controllers
{
    /// <summary>
    /// Controller to handle all event input and output.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class EventController(ApplicationDbContext applicationDbContext, IWebHostEnvironment environment) : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext = applicationDbContext;
        private readonly IWebHostEnvironment _environment = environment;

        /// <summary>
        /// Gets a paginated list of events, ordered by start date.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string title = "")
        {
            try
            {
                page = Math.Max(page, 1);

                int skip = (page - 1) * pageSize;

                IQueryable<Event> query = _applicationDbContext.Events.AsQueryable();

                if (!string.IsNullOrWhiteSpace(title))
                    query = query.Where(e => e.Title.Contains(title));

                List<Event> events = await query
                    .OrderBy(e => e.StartDate)
                    .Skip(skip)
                    .Take(pageSize)
                    .ToListAsync();
                int totalCount = await _applicationDbContext.Events.CountAsync();
                int totalPages = (int) Math.Ceiling(totalCount / (double) pageSize);

                return StatusCode(201, ApiResponse<EventListDto>.Ok(new EventListDto { TotalPages = totalPages, Events = [.. events.Select(EventMapper.ToResponseDTO)] }));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<EventListDto>.Fail($"Internal Server Error: {ex.Message}"));
            }
        }

        /// <summary>
        /// Gets a single event by its ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                Event? eventItem = await _applicationDbContext.Events.FindAsync(id);

                if (eventItem == null)
                {
                    return NotFound(ApiResponse<Object>.Fail("Event not found"));
                }

                return StatusCode(201, ApiResponse<EventDTO>.Ok(EventMapper.ToResponseDTO(eventItem)));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, data = (object)null, error = $"Internal Server Error: {ex.Message}" });
            }
        }

        /// <summary>
        /// Creates a new event with the provided data. Handles file upload and returns the created event.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Store([FromForm] EventCreateDto dto)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ApiResponse<Object>.Fail("Bad Request: Invalid Data"));

                string logoPath = await HandleLogoUpload(dto);

                Event newEvent = new Event
                {
                    Title = dto.Title,
                    StartDate = dto.StartDate,
                    EndDate = dto.EndDate,
                    Location = dto.Location,
                    Description = dto.Description,
                    MainColorHex = dto.MainColorHex,
                    AccentColorHex = dto.AccentColorHex,
                    LogoPath = logoPath
                };

                _applicationDbContext.Events.Add(newEvent);
                await _applicationDbContext.SaveChangesAsync();

                return StatusCode(201, new
                {
                    success = true,
                    data = newEvent,
                    error = (object)null
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<Object>.Fail($"Internal Server Error: {ex.Message}"));
            }
        }

        /// <summary>
        /// Updates an existing event with the provided data. Handles file upload and returns the updated event.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] EventUpdateDTO dto)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(new { success = false, data = ModelState, error = "Bad Request: Invalid Data" });

                Event? eventItem = await _applicationDbContext.Events.FindAsync(id);

                if (eventItem == null)
                {
                    return NotFound(ApiResponse<Object>.Fail("Event not found"));
                }

                string logoPath = eventItem.LogoPath;
                if (dto.LogoFile != null)
                {
                    if(!string.IsNullOrEmpty(eventItem.LogoPath))
                    {
                        DeleteLogo(eventItem.LogoPath);
                    } 
                    logoPath = await HandleLogoUpload(dto);
                }

                eventItem.Title = dto.Title;
                eventItem.StartDate = dto.StartDate;
                eventItem.EndDate = dto.EndDate;
                eventItem.Location = dto.Location;
                eventItem.Description = dto.Description;
                eventItem.MainColorHex = dto.MainColorHex;
                eventItem.AccentColorHex = dto.AccentColorHex;
                eventItem.LogoPath = logoPath;

                await _applicationDbContext.SaveChangesAsync();

                return Ok(ApiResponse<EventDTO>.Ok(EventMapper.ToResponseDTO(eventItem)));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<Object>.Fail($"Internal Server Error: {ex.Message}"));
            }
        }

        /// <summary>
        /// Publishes an event. Returns nothing.
        /// </summary>
        [HttpPost("{id}/publish")]
        public async Task<IActionResult> Publish(int id)
        {
            try
            {
                Event? eventItem = await _applicationDbContext.Events.FindAsync(id);

                if (eventItem == null)
                {
                    return NotFound(ApiResponse<Object>.Fail("Event not found"));
                }

                eventItem.IsPublished = true;

                await _applicationDbContext.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<Object>.Fail(ex.ToString()));
            }
        }

        /// <summary>
        /// Unpublishes an event. Returns nothing.
        /// </summary>
        [HttpPost("{id}/unpublish")]
        public async Task<IActionResult> UnPublish(int id)
        {
            try
            {
                Event? eventItem = await _applicationDbContext.Events.FindAsync(id);

                if (eventItem == null) 
                { 
                    return NotFound(ApiResponse<Object>.Fail("Event not found"));
                }

                eventItem.IsPublished = false;

                await _applicationDbContext.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<Object>.Fail(ex.ToString()));
            }
        }

        /// <summary>
        /// Method to handle logo file upload.
        /// </summary>
        public async Task<string> HandleLogoUpload(IEventFileDTO dto)
        {
            if (dto.LogoFile == null)
                return string.Empty;

            try
            {
                string fileName = Guid.NewGuid() + Path.GetExtension(dto.LogoFile.FileName);
                string filePath = Path.Combine(_environment.WebRootPath, "images", "events", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.LogoFile.CopyToAsync(stream);
                }

                return $"/images/{fileName}";
            }
            catch (Exception ex)
            {
                throw new Exception($"Error uploading logo: {ex.Message}");
                Console.WriteLine($"Error uploading logo: {ex.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// Method to delete a logo file. Could be saved in a service in the future.
        /// </summary>
        public void DeleteLogo(string logoPath)
        {
            if (string.IsNullOrEmpty(logoPath))
                return;
            try
            {
                string existingFilePath = Path.Combine(_environment.WebRootPath, logoPath.TrimStart('/'));
                if (System.IO.File.Exists(existingFilePath))
                {
                    System.IO.File.Delete(existingFilePath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting logo: {ex.Message}");
            }
        }
    }
}
