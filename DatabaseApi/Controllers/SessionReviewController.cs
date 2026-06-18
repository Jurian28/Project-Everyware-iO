using DatabaseApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedClassLibrary.DTOs.Sessions;
using System.Security.Claims;

namespace DatabaseApi.Controllers
{

    [ApiController]
    [Route("sessions/{sessionId}/reviews")]
    public class SessionReviewController : Controller
    {
        private readonly ApplicationDbContext _context;
        public SessionReviewController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<ApiResponse<SessionReviewsResponseDTO>>> GetSessionReviewsForSession(int sessionId)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(ApiResponse<SessionReviewsResponseDTO>.Fail("User is not authenticated."));

            User_has_Session? userSession = _context.User_has_Sessions
                .FirstOrDefault(us => us.IdUser == userId && us.IdSession == sessionId);

            if (userSession == null)
                return StatusCode(403,
                    ApiResponse<SessionReviewsResponseDTO>.Fail("You do not have access to this session."));

            List<SessionReviewDTO> reviews = _context.SessionReviews
                .Where(sr => sr.IdSession == sessionId)
                .Select(sr => new SessionReviewDTO
                {
                    Rating = sr.Rating,
                    Comment = sr.Comment
                })
                .ToList();

            SessionReviewsResponseDTO response = new()
            {
                CanReview = userSession.IsAttending,
                Reviews = reviews
            };

            return Ok(ApiResponse<SessionReviewsResponseDTO>.Ok(response));
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ApiResponse<SessionReviewDTO>>> ReviewSession(
            int sessionId,
            [FromBody] SessionReviewDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(
                    ApiResponse<SessionReviewDTO>.Fail("Invalid request data."));

            Session? session = _context.Sessions
                .FirstOrDefault(s => s.IdSession == sessionId);

            if (session == null)
                return NotFound(
                    ApiResponse<SessionReviewDTO>.Fail("Session not found."));

            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(
                    ApiResponse<SessionReviewDTO>.Fail("User is not authenticated."));

            User_has_Session? userSession = _context.User_has_Sessions
                .FirstOrDefault(us => us.IdUser == userId && us.IdSession == sessionId);

            if (userSession == null)
                return StatusCode(403,
                    ApiResponse<SessionReviewDTO>.Fail("You do not have access to this session."));

            if (!userSession.IsAttending)
                return BadRequest(
                    ApiResponse<SessionReviewDTO>.Fail("You must attend this session before reviewing it."));

            SessionReview review = new SessionReview()
            {
                IdSession = sessionId,
                Rating = dto.Rating,
                Comment = dto.Comment
            };

            _context.SessionReviews.Add(review);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse<SessionReviewDTO>.Ok(dto));
        }
    }
}
