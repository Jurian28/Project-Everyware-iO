using DatabaseApi.Models;
using DatabaseApi.Models.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatabaseApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventController(ApplicationDbContext applicationDbContext, IWebHostEnvironment environment): Controller
    {
        private readonly ApplicationDbContext _applicationDbContext = applicationDbContext;
        private readonly IWebHostEnvironment _environment = environment;

        [HttpGet("")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                List<Event> events = await _applicationDbContext.Events.ToListAsync();

                return StatusCode(201, new
                {
                    success = true,
                    data = events,
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
            } catch(Exception ex)
            {
                return StatusCode(500, new { success = false, data = (object)null, error = $"Internal Server Error: {ex.Message}" });
            }
        }

        public async Task<string> HandleLogoUpload(EventCreateDto dto)
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
    }
}
