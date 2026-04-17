using DatabaseApi.DTOs.Events;
using DatabaseApi.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatabaseApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventController(ApplicationDbContext applicationDbContext, IWebHostEnvironment environment) : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext = applicationDbContext;
        private readonly IWebHostEnvironment _environment = environment;

        [HttpGet("")]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                page = Math.Max(page, 1);
                page = Math.Min(page, 100);

                int skip = (page - 1) * pageSize;

                List<Event> events = await _applicationDbContext.Events
                                                .OrderBy(e => e.StartDate)
                                                .Skip(skip)
                                                .Take(pageSize)
                                                .ToListAsync();
                int totalCount = await _applicationDbContext.Events.CountAsync();
                int totalPages = (int) Math.Ceiling(totalCount / (double) pageSize);

                return StatusCode(201, new
                {
                    success = true,
                    data = new { events, totalPages },
                    error = (object)null
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, data = (object)null, error = $"Internal Server Error: {ex.Message}" });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                Event? eventItem = await _applicationDbContext.Events.FindAsync(id);

                if (eventItem == null)
                {
                    return NotFound(new { success = false, data = (object)null, error = "Event not found" });
                }

                return StatusCode(201, new
                {
                    success = true,
                    data = eventItem,
                    error = (object)null
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, data = (object)null, error = $"Internal Server Error: {ex.Message}" });
            }
        }

        [HttpPost("")]
        public async Task<IActionResult> Store([FromForm] EventCreateDto dto)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(new { success = false, data = ModelState, error = "Bad Request: Invalid Data" });

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
                return StatusCode(500, new { success = false, data = (object)null, error = $"Internal Server Error: {ex.Message}" });
            }
        }
        
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] EventUpdateDto dto)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(new { success = false, data = ModelState, error = "Bad Request: Invalid Data" });

                Event? eventItem = await _applicationDbContext.Events.FindAsync(id);

                if (eventItem == null)
                {
                    return NotFound(new { success = false, data = (object)null, error = "Event not found" });
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

                return Ok(new
                {
                    success = true,
                    data = eventItem,
                    error = (object)null
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, data = (object)null, error = $"Internal Server Error: {ex.Message}" });
            }
        }

        [HttpPost("{id}/publish")]
        public async Task<IActionResult> Publish(int id, [FromBody] PublishEventDto dto)
        {
            try
            {
                Event? eventItem = await _applicationDbContext.Events.FindAsync(id);

                if (eventItem == null)
                {
                    return NotFound(new { success = false, data = (object)null, error = "Event not found" });
                }

                eventItem.IsPublished = dto.Publish;

                await _applicationDbContext.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    data = new { publish = eventItem.IsPublished },
                    error = (object)null
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, data = (object)null, error = $"Internal Server Error: {ex.Message}" });
            }
        }

        public async Task<string> HandleLogoUpload(EventFileDto dto)
        {
            if (dto.LogoFile == null)
                return string.Empty;

            try
            {
                string fileName = Guid.NewGuid() + Path.GetExtension(dto.LogoFile.FileName);
                string filePath = Path.Combine(_environment.WebRootPath, "images", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.LogoFile.CopyToAsync(stream);
                }

                return $"/images/{fileName}";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading logo: {ex.Message}");
                return string.Empty;
            }
        }

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
