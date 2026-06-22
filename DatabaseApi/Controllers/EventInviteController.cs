using DatabaseApi.Models;
using DatabaseApi.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatabaseApi.Controllers;

/// <summary>
/// Handles the creation and acceptance of event invitations.
/// </summary>
/// <param name="applicationDbContext">The application database context.</param>
[ApiController]
[Authorize]
[Route("api/invites")]
public class EventInviteController(ApplicationDbContext applicationDbContext) : Controller
{
    private readonly ApplicationDbContext _applicationDbContext = applicationDbContext;

    private static string GenerateToken()
    {
        const string chars = "ABCDEFGHJKMNPQRSTUVWXYZ23456789";
        Random rng = new Random();
        string part1 = new string(Enumerable.Range(0, 4).Select(_ => chars[rng.Next(chars.Length)]).ToArray());
        string part2 = new string(Enumerable.Range(0, 4).Select(_ => chars[rng.Next(chars.Length)]).ToArray());
        return $"{part1}-{part2}";
    }

    /// <summary>
    /// Creates a new invitation for a specific event.
    /// </summary>
    /// <param name="eventInviteCreateDto">The data transfer object containing the event ID and expiration date.</param>
    /// <returns>An action result indicating success or failure, with the invite ID if successful.</returns>
    [HttpPost("create")]
    public async Task<IActionResult> CreateInvite([FromBody] EventInviteCreateDto eventInviteCreateDto)
    {
        Event? @event = await _applicationDbContext.Events.FindAsync(eventInviteCreateDto.EventId);

        if (@event == null)
        {
            return BadRequest(new
            {
                Success = false,
                Data = (object?)null,
                Error = "Event not found.",
            });
        }

        if (eventInviteCreateDto.Expires.Date < DateTime.UtcNow.Date)
        {
            return BadRequest(new
            {
                Success = false,
                Data = (object?)null,
                Error = "Expiration date must be in the future."
            });
        }

        string token;
        do
        {
            token = GenerateToken();
        }
        while (await _applicationDbContext.EventInvites.AnyAsync(i => i.Token == token));

        EventInvite invite = new()
        {
            Token = token,
            Event = @event,
            Expires = eventInviteCreateDto.Expires.ToUniversalTime()
        };
        _applicationDbContext.EventInvites.Add(invite);
        await _applicationDbContext.SaveChangesAsync();

        return StatusCode(201, new
        {
            Success = true,
            Data = new {
                Token = invite.Token
            },
            Error = (string?)null
        });
    }

    /// <summary>
    /// Accepts a pending event invitation.
    /// </summary>
    /// <param name="token">The short invite token (format XXXX-XXXX) identifying the invitation.</param>
    /// <returns>An action result indicating whether the invitation was successfully accepted.</returns>
    [HttpPost("accept/{token}")]
    public async Task<IActionResult> AcceptInvite(string token)
    {
        EventInvite? invite = await _applicationDbContext.EventInvites
            .Include(invite => invite.Event)
            .FirstOrDefaultAsync(invite => invite.Token == token.ToUpper());

        if (invite == null)
        {
            return BadRequest(new
            {
                Success = false,
                Data = (object?)null,
                Error = "Invite not found."
            });
        }

        if (invite.Expires.Date < DateTime.UtcNow.Date)
        {
            _applicationDbContext.EventInvites.Remove(invite);
            await _applicationDbContext.SaveChangesAsync();

            return BadRequest(new
            {
                Success = false,
                Data = (object?)null,
                Error = "Invite has expired."
            });
        }

        User? user = _applicationDbContext.Users
            .Include(user => user.Events)
            .FirstOrDefault(user => user.UserName == User.Identity.Name);

        if (user is null)
        {
            return BadRequest(new
            {
                Success = false,
                Data = (object?)null,
                Error = "User not found."
            });
        }

        if (user.Events.Any(item => item.IdEvent == invite.Event.IdEvent))
        {
            return BadRequest(new
            {
                Success = false,
                Data = (object?)null,
                Error = "You already joined that event."
            });
        }

        user.Events.Add(invite.Event);
        await _applicationDbContext.SaveChangesAsync();

        return Ok(new
        {
            Success = true,
            Data = (object?)null,
            Error = (string?)null,
        });
    }
}