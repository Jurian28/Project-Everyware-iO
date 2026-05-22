using SharedClassLibrary.DTOs.Events;
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
        public async Task<IActionResult> CheckAttendance()
        {
            try
            {
                // TODO, check attendance
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<Object>.Fail($"CheckAttendance: Internal Server Error: {ex.Message}"));
            }
        }
    }
}