using System.Security.Claims;
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
            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            List<PollDTO> polls = await _context.Polls
                .Where(p => p.IdSession == sessionId)
                .Include(p => p.Answers)
                    .ThenInclude(a => a.Votes)
                .Select(p => new PollDTO
                {
                    IdPoll = p.IdPoll,
                    Title = p.Title,
                    Description = p.Description,
                    IsClosed = p.IsClosed,
                    IdSession = p.IdSession,
                    HasVoted = userId != null && p.Answers.Any(a => a.Votes.Any(v => v.IdUser == userId)),
                    VotedAnswerId = userId != null
                        ? p.Answers.SelectMany(a => a.Votes).Where(v => v.IdUser == userId).Select(v => (int?)v.IdPollAnswer).FirstOrDefault()
                        : null,
                    Answers = p.Answers.Select(a => new PollAnswerDTO
                    {
                        IdPollAnswer = a.IdPollAnswer,
                        Text = a.Text,
                        VoteCount = a.Votes.Count
                    }).ToList()
                })
                .ToListAsync();

            return Ok(ApiResponse<IEnumerable<PollDTO>>.Ok(polls));
        }

        [HttpPost]
        [Authorize(Roles = "Speaker, Admin")]
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
        [Authorize(Roles = "Speaker, Admin")]
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
        [Authorize(Roles = "Speaker, Admin")]
        public async Task<ActionResult<ApiResponse<PollDTO>>> Close(int id)
        {
            Poll? poll = await _context.Polls
                .Include(p => p.Answers)
                    .ThenInclude(a => a.Votes)
                .FirstOrDefaultAsync(p => p.IdPoll == id);

            if (poll == null)
                return NotFound(ApiResponse<PollDTO>.Fail("Poll not found."));

            try
            {
                poll.IsClosed = true;
                await _context.SaveChangesAsync();

                string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                return Ok(ApiResponse<PollDTO>.Ok(PollMapper.ToResponseDTO(poll, userId)));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, ApiResponse<PollDTO>.Fail("Unexpected error"));
            }
        }

        [HttpPost("{id}/vote")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<PollDTO>>> Vote(int id, [FromBody] PollVoteDTO dto)
        {
            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized(ApiResponse<PollDTO>.Fail("User not authenticated."));

            Poll? poll = await _context.Polls
                .Include(p => p.Answers)
                    .ThenInclude(a => a.Votes)
                .FirstOrDefaultAsync(p => p.IdPoll == id);

            if (poll == null)
                return NotFound(ApiResponse<PollDTO>.Fail("Poll not found."));

            if (poll.IsClosed)
                return BadRequest(ApiResponse<PollDTO>.Fail("Poll is closed."));

            if (!poll.Answers.Any(a => a.IdPollAnswer == dto.IdPollAnswer))
                return BadRequest(ApiResponse<PollDTO>.Fail("Answer not found in this poll."));

            if (poll.Answers.Any(a => a.Votes.Any(v => v.IdUser == userId)))
                return BadRequest(ApiResponse<PollDTO>.Fail("You have already voted on this poll."));

            try
            {
                PollVote vote = new PollVote
                {
                    IdUser = userId,
                    IdPoll = id,
                    IdPollAnswer = dto.IdPollAnswer
                };

                _context.PollVotes.Add(vote);
                await _context.SaveChangesAsync();

                PollDTO response = new PollDTO
                {
                    IdPoll = poll.IdPoll,
                    Title = poll.Title,
                    Description = poll.Description,
                    IsClosed = poll.IsClosed,
                    IdSession = poll.IdSession,
                    HasVoted = true,
                    VotedAnswerId = dto.IdPollAnswer,
                    Answers = poll.Answers.Select(a => new PollAnswerDTO
                    {
                        IdPollAnswer = a.IdPollAnswer,
                        Text = a.Text,
                        VoteCount = a.Votes.Count
                    }).ToList()
                };

                return Ok(ApiResponse<PollDTO>.Ok(response));
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, ApiResponse<PollDTO>.Fail("Database error."));
            }
        }
    }
}
