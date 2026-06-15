using DatabaseApi.Models;
using DatabaseApi.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SharedClassLibrary.DTOs.Auth;
using SharedClassLibrary.Jwt;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;
using User = DatabaseApi.Models.User;

namespace DatabaseApi.Controllers;

/// <summary>
/// Controller responsible for handling Organiser management.
/// </summary>
[ApiController]
[Route("[controller]")]
public class EventorganiserController(UserManager<User> userManager, ApplicationDbContext applicationDbContext) : Controller
{
    private static readonly string INCOMPLETE_CREDENTIALS_MESSAGE = "Email and password are required.";
    private static readonly string INVALID_CREDENTIALS_MESSAGE = "Invalid email or password.";
    private static readonly string ACCOUNT_ALREADY_EXISTS_MESSAGE = "An account with this email already exists.";

    private readonly UserManager<User> _userManager = userManager;
    private readonly ApplicationDbContext _applicationDbContext = applicationDbContext;

    [HttpGet("organisers")]
    [Authorize(Roles = "Admin")]
    public async Task<ApiResponse<Object>> GetOrganisers()
    {
        IList<User> organisers = await _userManager.GetUsersInRoleAsync("Organiser");

        return ApiResponse<Object>.Ok(organisers.Select(u => new UserDTO
        {
            Id = u.Id,
            Email = u.Email,
        }));
    }

    [HttpGet("organiser-requests")]
    [Authorize(Roles = "Admin")]
    public async Task<ApiResponse<Object>> GetOrganiserRequests()
    {
        List<User> requesters = await _applicationDbContext.Users
            .Where(u => u.HasRequestedAccess)
            .ToListAsync();

        List<User> filtered = [];
        foreach (var user in requesters)
        {
            if (!await _userManager.IsInRoleAsync(user, "Organiser")) { filtered.Add(user); }
        }

        return ApiResponse<Object>.Ok(filtered.Select(u => new UserDTO
        {
            Id = u.Id,
            Email = u.Email,
        }));
    }

    /// <summary>
    /// Handles requesting acces as a user without access.
    /// </summary>
    [HttpPost("request-organiser-access")]
    [Authorize]
    public async Task<IActionResult> RequestAccess()
    {
        string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId)) { return BadRequest(ApiResponse<Object>.Fail("Name identifier not found.")); }

        User? user = await _applicationDbContext.Users.FindAsync(userId);

        if (user == null) { return NotFound(ApiResponse<Object>.Fail("User not found.")); }
        if (user.HasRequestedAccess) { return BadRequest(ApiResponse<Object>.Fail("You have already requested access.")); }

        user.HasRequestedAccess = true;
        await _applicationDbContext.SaveChangesAsync();

        return Ok(ApiResponse<Object>.Ok(null));
    }

    /// <summary>
    /// Handles requesting acces as a user without access.
    /// </summary>
    [HttpGet("has-requested-organiser-access")]
    [Authorize]
    public async Task<IActionResult> HasRequestedAccess()
    {
        try
        {
            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId)) { return BadRequest(ApiResponse<Object>.Fail("Name identifier not found.")); }
            User? user = await _applicationDbContext.Users.FindAsync(userId);

            if (user == null) { return NotFound(ApiResponse<Object>.Fail("User not found.")); }

            return Ok(ApiResponse<bool>.Ok(user.HasRequestedAccess));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, ApiResponse<bool>.Fail("Error Occurred"));
        }
    }

    /// <summary>
    /// Handles removing the request for the organiser role from a user or all users.
    /// </summary>
    [HttpPost("remove-organiser-request/{userId?}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RemoveRequest(string? userId)
    {
        try
        {
            if (userId != null)
            {
                User? user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    return NotFound(ApiResponse<Object>.Fail("User not found."));
                }

                user.HasRequestedAccess = false;
            }
            else
            {
                await _applicationDbContext.Users
                    .Where(u => u.HasRequestedAccess)
                    .ExecuteUpdateAsync(s => s.SetProperty(u => u.HasRequestedAccess, false));
            }

            await _applicationDbContext.SaveChangesAsync();
            return Ok(ApiResponse<Object>.Ok(null));
        }
        catch (Exception err)
        {
            return StatusCode(500, (ApiResponse<Object>.Fail(err.ToString())));
        }
    }


    /// <summary>
    /// Handles revoking the organiser role from a user.
    /// </summary>
    /// <param name="userId">The id of the user to revoke the organiser role from.</param>
    /// <returns>An HTTP response indicating whether the role was successfully revoked.</returns>
    [HttpPost("revoke-organiser/{userId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RevokeOrganiser(string userId)
    {
        User? user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return NotFound(ApiResponse<Object>.Fail("User not found."));
        }

        if (!await _userManager.IsInRoleAsync(user, "Organiser"))
        {
            return BadRequest(ApiResponse<Object>.Fail("User does not have the Organiser role."));
        }

        IdentityResult result = await _userManager.RemoveFromRoleAsync(user, "Organiser");

        if (!result.Succeeded)
        {
            return BadRequest(ApiResponse<Object>.Fail(string.Join(" ", result.Errors.Select(e => e.Description))));
        }

        return Ok(ApiResponse<Object>.Ok(null));
    }

    /// <summary>
    /// Returns all users who have the Speaker role.
    /// </summary>
    [HttpGet("speakers")]
    [Authorize(Roles = "Admin")]
    public async Task<ApiResponse<Object>> GetSpeakers()
    {
        IList<User> speakers = await _userManager.GetUsersInRoleAsync("Speaker");

        return ApiResponse<Object>.Ok(speakers.Select(u => new UserDTO
        {
            Id = u.Id,
            Email = u.Email,
        }));
    }

    /// <summary>
    /// Removes the Speaker role from a user.
    /// </summary>
    [HttpPost("revoke-speaker/{userId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RevokeSpeaker(string userId)
    {
        User? user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return NotFound(ApiResponse<Object>.Fail("User not found."));
        }

        if (!await _userManager.IsInRoleAsync(user, "Speaker"))
        {
            return BadRequest(ApiResponse<Object>.Fail("User does not have the Speaker role."));
        }

        IdentityResult result = await _userManager.RemoveFromRoleAsync(user, "Speaker");

        if (!result.Succeeded)
        {
            return BadRequest(ApiResponse<Object>.Fail(string.Join(" ", result.Errors.Select(e => e.Description))));
        }

        return Ok(ApiResponse<Object>.Ok(null));
    }

    /// <summary>
    /// Generic endpoint to instate a user into any role.
    /// </summary>
    [HttpPost("instate-role/{userId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> InstateRole(string userId, [FromBody] RoleAssignmentDto dto)
    {
        User? user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return NotFound(ApiResponse<Object>.Fail("User not found."));
        }

        if (await _userManager.IsInRoleAsync(user, dto.Role))
        {
            return StatusCode(400, ApiResponse<Object>.Fail($"User already has the {dto.Role} role."));
        }

        IdentityResult result = await _userManager.AddToRoleAsync(user, dto.Role);

        if (!result.Succeeded)
        {
            return StatusCode(500, ApiResponse<Object>.Fail(string.Join(" ", result.Errors.Select(e => e.Description))));
        }

        user.HasRequestedAccess = false;
        await _applicationDbContext.SaveChangesAsync();

        return Ok(ApiResponse<Object>.Ok(null));
    }
}