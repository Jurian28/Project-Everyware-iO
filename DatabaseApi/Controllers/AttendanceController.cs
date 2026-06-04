using SharedClassLibrary.DTOs.Sessions;
using DatabaseApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using DatabaseApi.DTOs;
using System.Net;

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
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(ApiResponse<Object>.Fail("UserId is required"));
                }

                if (sessionId <= 0)
                {
                    return BadRequest(ApiResponse<Object>.Fail("Invalid SessionId"));
                }

                var attendance = await _applicationDbContext.SessionAttendances
                    .FirstOrDefaultAsync(sa => sa.IdSession == sessionId && sa.UserId == userId);

                if (attendance != null)
                {
                    return StatusCode(409, ApiResponse<Object>.Fail("User is already attending this session"));
                }
                else
                {
                    await _applicationDbContext.SessionAttendances.AddAsync(new SessionAttendance
                    {
                        UserId = userId,
                        IdSession = sessionId
                    });
                }

                await _applicationDbContext.SaveChangesAsync();

                return Ok(ApiResponse<SessionAttendenceDTO>.Ok(new SessionAttendenceDTO
                {
                    UserId = userId,
                    SessionId = sessionId
                }));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<Object>.Fail($"Internal Server Error: {ex.Message}"));
            }
        }
    }
}