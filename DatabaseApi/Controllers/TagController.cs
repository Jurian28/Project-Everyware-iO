using DatabaseApi.DTOs.Tags;
using DatabaseApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SharedClassLibrary.ApiResponse;


namespace DatabaseApi.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class TagController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TagController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        // [Authorize]
        public async Task<ActionResult<ApiResponse<IEnumerable<TagResponseDTO>>>> GetAllTags([FromQuery] int? eventId)
        {
            IQueryable<Tag> query = _context.Tags;

            if (eventId.HasValue)
                query = query.Where(t => t.IdEvent == eventId.Value);

            var tags = await query
                .Select(t => new TagResponseDTO
                {
                    IdEvent = t.IdEvent,
                    Title = t.Title,
                    ColorHex = t.ColorHex
                })
                .ToListAsync();

            return Ok(ApiResponse<IEnumerable<TagResponseDTO>>.Ok(tags));
        }


        [HttpPost]
        // [Authorize]
        public async Task<ActionResult<ApiResponse<TagResponseDTO>>> InsertTag([FromBody] TagInsertDTO dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(e => e.Value.Errors.Count > 0)
                    .ToDictionary(
                        e => e.Key,
                        e => e.Value.Errors.Select(err => err.ErrorMessage).ToArray()
                    );

                return BadRequest(ApiResponse<object>.Fail("Validation failed", errors));
            }

            try
            {
                var tag = TagMapper.ToEntity(dto);

                _context.Tags.Add(tag);
                await _context.SaveChangesAsync();

                var response = TagMapper.ToResponseDTO(tag);

                return StatusCode(201, ApiResponse<TagResponseDTO>.Ok(response));
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx)
            {
                return sqlEx.Number switch
                {
                    547 => NotFound(ApiResponse<TagResponseDTO>.Fail($"Event {dto.IdEvent} not found.")),
                    2601 => Conflict(ApiResponse<TagResponseDTO>.Fail("Tag already exists for this event.")),
                    2627 => Conflict(ApiResponse<TagResponseDTO>.Fail("Tag already exists for this event.")),
                    _ => StatusCode(500, ApiResponse<TagResponseDTO>.Fail("Database error"))
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, ApiResponse<TagResponseDTO>.Fail("Unexpected error"));
            }
        }


        [HttpPut("{idEvent}/{title}")]
        public async Task<ActionResult<ApiResponse<TagResponseDTO>>> UpdateTag(int idEvent, string title, [FromBody] TagUpdateDTO dto)
        {
            var tag = await _context.Tags
                .FirstOrDefaultAsync(t => t.IdEvent == idEvent && t.Title == title);

            if (tag == null)
                return NotFound(ApiResponse<TagResponseDTO>.Fail("Tag not found."));

            try
            {
                _context.Remove(tag);
                _context.SaveChanges();
                Tag newTag = new Tag
                {
                    IdEvent = idEvent,
                    Title = dto.Title,
                    ColorHex = dto.ColorHex
                };
                _context.Tags.Add(newTag);

                // TagMapper.UpdateEntity(tag, dto);
                await _context.SaveChangesAsync();

                return Ok(ApiResponse<TagResponseDTO>.Ok(TagMapper.ToResponseDTO(newTag)));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, ApiResponse<TagResponseDTO>.Fail("Unexpected error"));
            }
        }
        [HttpDelete("{idEvent}/{title}")]
        public async Task<IActionResult> DeleteTag(int idEvent, string title)
        {
            var tag = await _context.Tags
                .FirstOrDefaultAsync(t => t.IdEvent == idEvent && t.Title == title);

            if (tag == null)
                return NotFound(ApiResponse<TagResponseDTO>.Fail("Tag not found."));

            try
            {
                _context.Tags.Remove(tag);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, ApiResponse<TagResponseDTO>.Fail("Unexpected database error."));
            }
        }
    }
}
