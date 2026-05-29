using SharedClassLibrary.DTOs.Sessions;
using DatabaseApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using DatabaseApi.DTOs;

namespace DatabaseApi.Controllers
{
    /// <summary>
    /// Controller to handle all attendance input and output.
    /// </summary>
    [ApiController]
    [Route("sessions/{sessionId}/[controller]")]
    public class AttendanceController(ApplicationDbContext applicationDbContext, IWebHostEnvironment environment) : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext = applicationDbContext;
        private readonly IWebHostEnvironment _environment = environment;

        /// <summary>
        /// Gets a paginated list of events, ordered by start date.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CheckAttendance([FromRoute] int sessionId, [FromBody] string userId)
        {
            try
            {
                Console.WriteLine($"[Controller] Received attendance check for session {sessionId} from user {userId}");
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(ApiResponse<Object>.Fail("UserId is required"));
                }

                if (sessionId <= 0)
                {
                    return BadRequest(ApiResponse<Object>.Fail("Invalid SessionId"));
                }

                await _applicationDbContext.SessionAttendances.AddAsync(new SessionAttendance
                {
                    UserId = userId,
                    IdSession = sessionId,
                    IsAttending = true
                });
                await _applicationDbContext.SaveChangesAsync();

                Console.WriteLine($"User {userId} marked as attending session {sessionId}");
                Console.WriteLine($"User {userId} marked as attending session {sessionId}");
                Console.WriteLine($"User {userId} marked as attending session {sessionId}");
                Console.WriteLine("");

                // TODO, check attendance
                return Ok(ApiResponse<SessionAttendenceDTO>.Ok(new SessionAttendenceDTO
                {
                    UserId = userId,
                    SessionId = sessionId,
                    IsAttending = true
                }));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<Object>.Fail($"Internal Server Error: {ex.Message}"));
            }
        }
    }
}