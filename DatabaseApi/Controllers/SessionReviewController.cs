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
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            User_has_Session? userSession = _context.User_has_Sessions.Where(us => us.IdUser == userId).Where(us => us.IdSession == sessionId).FirstOrDefault();
            if (userSession == null) return Forbid();

            List<SessionReviewDTO> reviews = _context.SessionReviews
                .Where(sr => sr.IdSession == sessionId)
                .Select(sr => new SessionReviewDTO{
                    Rating = sr.Rating,
                    Comment = sr.Comment
                }).ToList();

            bool canReview = userSession.IsAttending;

            SessionReviewsResponseDTO response = new SessionReviewsResponseDTO
            {
                CanReview = canReview,
                Reviews = reviews
            };

            return Ok(ApiResponse<SessionReviewsResponseDTO>.Ok(response));
        }


        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ApiResponse<SessionReviewDTO>>> ReviewSession(int sessionId,[FromBody] SessionReviewDTO dto) {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            Session? session = _context.Sessions.Where(s => s.IdSession == sessionId).FirstOrDefault();
            if (session == null) return NotFound();

            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            User_has_Session? userSession = _context.User_has_Sessions
                .Where(us => us.IdUser == userId)
                .Where(us => us.IdSession == sessionId)
                .FirstOrDefault();

            if (userSession == null) return Forbid();

            if (!userSession.IsAttending)
                return BadRequest(ApiResponse<SessionReviewDTO>.Fail("You must attend this session before reviewing it."));

            SessionReview review = new SessionReview
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
