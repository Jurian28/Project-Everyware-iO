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
                    stars = sr.stars,
                    comment = sr.comment
                }).ToList();

            return Ok(ApiResponse<List<SessionReviewDTO>>.Ok(reviews));
        }


        [HttpPost]
        public async Task<ActionResult<ApiResponse<SessionReviewDTO>>> ReviewSession(int sessionId,[FromBody] SessionReviewDTO dto) {
            SessionReview review = new SessionReview
            {
                IdSession = sessionId,
                stars = dto.stars,
                comment = dto.comment
            };

            _context.SessionReviews.Add(review);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse<SessionReviewDTO>.Ok(dto));
        }
    }
}
