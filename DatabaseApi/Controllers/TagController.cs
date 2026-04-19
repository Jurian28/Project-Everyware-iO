using DatabaseApi.DTOs.Tags;
using DatabaseApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace DatabaseApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    /// <summary>
    /// Controller to handle all tag input and output.
    /// </summary>
    public class TagController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TagController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets all tags. If eventId is provided, it returns only tags for that event; otherwise, it returns all tags in the database.
        /// </summary>
        [HttpGet]
        // [Authorize]
        public async Task<ActionResult<ApiResponse<IEnumerable<TagResponseDTO>>>> GetAllTags([FromQuery] int? eventId)
        {
            IQueryable<Tag> query = _context.Tags;

            if (eventId.HasValue)
                query = query.Where(t => t.IdEvent == eventId.Value);

            List<TagResponseDTO> tags = await query
                .Select(t => new TagResponseDTO
                {
                    IdTag = t.IdTag,
                    IdEvent = t.IdEvent,
                    Title = t.Title,
                    ColorHex = t.ColorHex
                })
                .ToListAsync();

            return Ok(ApiResponse<IEnumerable<TagResponseDTO>>.Ok(tags));
        }

        /// <summary>
        /// Inserts a new tag into the database. The tag data is provided in the request body as a TagInsertDTO.
        /// </summary>
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
                Tag? tag = TagMapper.ToEntity(dto);

                _context.Tags.Add(tag);
                await _context.SaveChangesAsync();

                TagResponseDTO response = TagMapper.ToResponseDTO(tag);

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

        /// <summary>
        /// Updates an existing tag in the database. The tag ID is provided as a route parameter, and the updated tag data is provided in the request body as a TagUpdateDTO.
        /// </summary>
        [HttpPut("{idTag}")]
        public async Task<ActionResult<ApiResponse<TagResponseDTO>>> UpdateTag(int idTag, [FromBody] TagUpdateDTO dto)
        {
            Tag? tag = await _context.Tags
                .FirstOrDefaultAsync(t => t.IdTag == idTag);

            if (tag == null)
                return NotFound(ApiResponse<TagResponseDTO>.Fail("Tag not found."));

            try
            {
                TagMapper.UpdateEntity(tag, dto);
                await _context.SaveChangesAsync();

                return Ok(ApiResponse<TagResponseDTO>.Ok(TagMapper.ToResponseDTO(tag)));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, ApiResponse<TagResponseDTO>.Fail("Unexpected error"));
            }
        }

        /// <summary>
        /// Deletes a tag from the database. The tag ID is provided as a route parameter.
        /// </summary>
        [HttpDelete("{idTag}")]
        public async Task<IActionResult> DeleteTag(int idTag)
        {
            Tag? tag = await _context.Tags
                .FirstOrDefaultAsync(t => t.IdTag == idTag);

            if (tag == null)
                return NotFound(ApiResponse<TagResponseDTO>.Fail("Tag not found."));

            try
            {
                _context.Tags.Remove(tag);
                await _context.SaveChangesAsync();

                return StatusCode(204, ApiResponse<TagResponseDTO>.Ok(null));
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, ApiResponse<TagResponseDTO>.Fail("Unexpected database error."));
            }
        }
    }
}
