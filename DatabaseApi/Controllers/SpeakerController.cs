using DatabaseApi.DTOs.Speakers;
using DatabaseApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SharedClassLibrary.ApiResponse;
namespace DatabaseApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    /// <summary>
    /// Controller to handle all speaker input and output.
    /// </summary>
    public class SpeakerController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public SpeakerController(ApplicationDbContext context)
        {
            _context = context;
        }
        /// <summary>
        /// gets all speakers, if eventId is provided it gets all speakers for that event, otherwise it gets all speakers in the database
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SpeakerResponseDTO>>> GetAllSpeakers([FromQuery] int? eventId)
        {
            IQueryable<Speaker> query = _context.Speakers;

            if (eventId.HasValue)
            {
                query = query.Where(r => r.IdEvent == eventId.Value);
            }

            var speakers = await query.ToListAsync();
            return Ok(ApiResponse<IEnumerable<SpeakerResponseDTO>>.Ok(speakers.Select(SpeakerMapper.ToResponseDTO)));
        }
        /// <summary>
        /// Inserts a new speaker into the database. The speaker data is provided in the request body as a SpeakerInsertDTO.
        /// </summary>

        [HttpPost]
        public async Task<ActionResult<ApiResponse<SpeakerResponseDTO>>> InsertSpeaker([FromBody] SpeakerInsertDTO speakerDTO)
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
                Speaker speaker = SpeakerMapper.ToEntity(speakerDTO);
                _context.Speakers.Add(speaker);
                await _context.SaveChangesAsync();

                var response = SpeakerMapper.ToResponseDTO(speaker);
                return StatusCode(201, ApiResponse<SpeakerResponseDTO>.Ok(response));
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx)
            {
                return sqlEx.Number switch
                {
                    547 => NotFound(ApiResponse<SpeakerResponseDTO>.Fail($"Event with ID {speakerDTO.IdEvent} not found.")),
                    _ => StatusCode(500, ApiResponse<SpeakerResponseDTO>.Fail("An unexpected database error occurred."))
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex}");
                return StatusCode(500, ApiResponse<SpeakerResponseDTO>.Fail("An unexpected error occurred."));
            }
        }
        /// <summary>
        /// Updates an existing speaker in the database. The speaker ID is provided as a route parameter, and the updated speaker data is provided in the request body as a SpeakerUpdateDTO.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<Speaker>> UpdateSpeaker(int id, [FromBody] SpeakerUpdateDTO speakerDTO)
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


            var speaker = await _context.Speakers.FindAsync(id);
            if (speaker == null) return NotFound();

            try
            {
                SpeakerMapper.UpdateEntity(speaker, speakerDTO);
                await _context.SaveChangesAsync();
                return Ok(ApiResponse<SpeakerResponseDTO>.Ok(SpeakerMapper.ToResponseDTO(speaker)));
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx)
            {
                return sqlEx.Number switch
                {
                    _ => StatusCode(500, ApiResponse<SpeakerResponseDTO>.Fail("An unexpected error occurred."))
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex}");
                return StatusCode(500, ApiResponse<SpeakerResponseDTO>.Fail("An unexpected error occurred."));
            }
        }
        /// <summary>
        /// Deletes a speaker from the database. The speaker ID is provided as a route parameter. If the speaker has associated sessions, it cannot be deleted and a conflict response is returned.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSpeaker(int id)
        {
            var speaker = await _context.Speakers.FindAsync(id);
            if (speaker == null) return NotFound(ApiResponse<SpeakerResponseDTO>.Fail("Speaker not found."));
            try
            {
                _context.Speakers.Remove(speaker);
                await _context.SaveChangesAsync();
                return StatusCode(204, ApiResponse<SpeakerResponseDTO>.Ok(null));
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx)
            {
                return sqlEx.Number switch
                {
                    547 => Conflict(ApiResponse<SpeakerResponseDTO>.Fail("This speaker cannot be deleted because it still has Sessions.")),
                    _ => StatusCode(500, ApiResponse<SpeakerResponseDTO>.Fail("An unexpected error occurred."))
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex}");
                return StatusCode(500, ApiResponse<SpeakerResponseDTO>.Fail("An unexpected error occurred."));
            }
        }
    }
}