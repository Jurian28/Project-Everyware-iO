using DatabaseApi.DTOs;
using SharedClassLibrary.DTOs.Polls;
using DatabaseApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace DatabaseApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PollController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PollController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("session/{sessionId}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<PollDTO>>>> GetAllBySession(int sessionId)
        {
            List<PollDTO> polls = await _context.Polls
                .Where(p => p.IdSession == sessionId)
                .Include(p => p.Answers)
                .Select(p => new PollDTO
                {
                    IdPoll = p.IdPoll,
                    Title = p.Title,
                    Description = p.Description,
                    IsClosed = p.IsClosed,
                    IdSession = p.IdSession,
                    Answers = p.Answers.Select(a => new PollAnswerDTO
                    {
                        IdPollAnswer = a.IdPollAnswer,
                        Text = a.Text
                    }).ToList()
                })
                .ToListAsync();

            return Ok(ApiResponse<IEnumerable<PollDTO>>.Ok(polls));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<PollDTO>>> GetById(int id)
        {
            Poll? poll = await _context.Polls
                .Include(p => p.Answers)
                .FirstOrDefaultAsync(p => p.IdPoll == id);

            if (poll == null)
                return NotFound(ApiResponse<PollDTO>.Fail("Poll not found."));

            return Ok(ApiResponse<PollDTO>.Ok(PollMapper.ToResponseDTO(poll)));
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ApiResponse<PollDTO>>> Create([FromBody] PollCreateDTO dto)
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
                Poll poll = PollMapper.ToEntity(dto);

                _context.Polls.Add(poll);
                await _context.SaveChangesAsync();

                PollDTO response = PollMapper.ToResponseDTO(poll);

                return StatusCode(201, ApiResponse<PollDTO>.Ok(response));
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx)
            {
                return sqlEx.Number switch
                {
                    547 => NotFound(ApiResponse<PollDTO>.Fail($"Session {dto.IdSession} not found.")),
                    _ => StatusCode(500, ApiResponse<PollDTO>.Fail("Database error"))
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, ApiResponse<PollDTO>.Fail("Unexpected error"));
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            Poll? poll = await _context.Polls
                .FirstOrDefaultAsync(p => p.IdPoll == id);

            if (poll == null)
                return NotFound(ApiResponse<PollDTO>.Fail("Poll not found."));

            try
            {
                _context.Polls.Remove(poll);
                await _context.SaveChangesAsync();

                return Ok(ApiResponse<PollDTO>.Ok(null));
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, ApiResponse<PollDTO>.Fail("Unexpected database error."));
            }
        }

        [HttpPost("{id}/close")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<PollDTO>>> Close(int id)
        {
            Poll? poll = await _context.Polls
                .Include(p => p.Answers)
                .FirstOrDefaultAsync(p => p.IdPoll == id);

            if (poll == null)
                return NotFound(ApiResponse<PollDTO>.Fail("Poll not found."));

            try
            {
                poll.IsClosed = true;
                await _context.SaveChangesAsync();

                return Ok(ApiResponse<PollDTO>.Ok(PollMapper.ToResponseDTO(poll)));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, ApiResponse<PollDTO>.Fail("Unexpected error"));
            }
        }
    }
}
