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
    [Route("[controller]")]
    public class AttendanceController(ApplicationDbContext applicationDbContext, IWebHostEnvironment environment) : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext = applicationDbContext;
        private readonly IWebHostEnvironment _environment = environment;

        /// <summary>
        /// Gets a paginated list of events, ordered by start date.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CheckAttendance([FromBody] SessionAttendenceDTO sessionAttendenceDTO)
        {
            try
            {
                sessionAttendenceDTO.UserId = sessionAttendenceDTO.UserId;

                if (string.IsNullOrEmpty(sessionAttendenceDTO.UserId))
                {
                    return Unauthorized(ApiResponse<Object>.Fail("UserId is required"));
                }

                if (sessionAttendenceDTO.SessionId <= 0)
                {
                    return BadRequest(ApiResponse<Object>.Fail("Invalid SessionId"));
                }

                await _applicationDbContext.SessionAttendances.AddAsync(new SessionAttendance
                {
                    UserId = sessionAttendenceDTO.UserId,
                    IdSession = sessionAttendenceDTO.SessionId,
                    IsAttending = sessionAttendenceDTO.IsAttending ?? true
                });
                await _applicationDbContext.SaveChangesAsync();

                // TODO, check attendance
                return Ok(ApiResponse<SessionAttendenceDTO>.Ok(new SessionAttendenceDTO
                {
                    UserId = sessionAttendenceDTO.UserId,
                    SessionId = sessionAttendenceDTO.SessionId,
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