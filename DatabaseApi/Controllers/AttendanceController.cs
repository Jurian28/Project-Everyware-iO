using SharedClassLibrary.DTOs.Sessions;
using DatabaseApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using DatabaseApi.DTOs;
using System.Net;
using DatabaseApi.Services;

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
        private readonly SessionRegistrationService _sessionRegistrationService = new(applicationDbContext);

        /// <summary>
        /// Checks if a user is attending a session and adds them to the attendance list if they are not already attending.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CheckAttendance([FromRoute] int sessionId, [FromBody] string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId)) return Unauthorized(ApiResponse<Object>.Fail("UserId is required"));
                
                if (sessionId <= 0) return BadRequest(ApiResponse<Object>.Fail("Invalid SessionId"));
                
                User? user = _applicationDbContext.Users
                    .Include(user => user.Events)
                    .FirstOrDefault(user => user.UserName == User.Identity!.Name);
                if (user == null) return BadRequest(ApiResponse<object>.Fail("User not found"));

                Session? session = await _sessionRegistrationService.GetSession(sessionId);
                if (session == null) return NotFound(ApiResponse<object>.Fail("The session you tried to register for does not exist."));

                User_has_Session? existingRegistration = await _sessionRegistrationService.GetExistingRegistration(user, session);
                if (existingRegistration == null) return BadRequest(ApiResponse<object>.Fail("You are not registered for this session"));

                SessionAttendance? attendance = await _applicationDbContext.SessionAttendances
                    .FirstOrDefaultAsync(sa => sa.IdSession == sessionId && sa.UserId == userId);

                if (attendance != null)
                    return StatusCode(409, ApiResponse<Object>.Fail("User is already attending this session"));
                
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