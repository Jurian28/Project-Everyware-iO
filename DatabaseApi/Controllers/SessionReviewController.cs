using DatabaseApi.Models;
using Microsoft.AspNetCore.Mvc;
using SharedClassLibrary.DTOs.Sessions;

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
        public async Task<ActionResult<ApiResponse<List<SessionReviewDTO>>>> GetSessionReviewsForSession(int sessionId)
        {
            List<SessionReviewDTO> reviews = _context.SessionReviews
                .Where(sr => sr.IdSession == sessionId)
                .Select(sr => new SessionReviewDTO{
                    Rating = sr.Rating,
                    Comment = sr.Comment
                }).ToList();

            return Ok(ApiResponse<List<SessionReviewDTO>>.Ok(reviews));
        }


        [HttpPost]
        public async Task<ActionResult<ApiResponse<SessionReviewDTO>>> ReviewSession(int sessionId,[FromBody] SessionReviewDTO dto) {
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
