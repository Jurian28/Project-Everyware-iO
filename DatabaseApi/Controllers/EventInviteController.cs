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

        if (eventInviteCreateDto.Expires < DateTime.UtcNow)
        {
            return BadRequest(new
            {
                Success = false,
                Data = (object?)null,
                Error = "Expiration date must be in the future."
            });
        }

        EventInvite invite = new()
        {
            Event = @event,
            Expires = eventInviteCreateDto.Expires
        };
        _applicationDbContext.EventInvites.Add(invite);
        await _applicationDbContext.SaveChangesAsync();

        return StatusCode(201, new
        {
            Success = true,
            Data = new {
                Invite = invite.Id
            },
            Error = (string?)null
        });
    }

    /// <summary>
    /// Accepts a pending event invitation.
    /// </summary>
    /// <param name="inviteId">The unique identifier of the invitation to accept.</param>
    /// <returns>An action result indicating whether the invitation was successfully accepted.</returns>
    [HttpPost("accept/{inviteId}")]
    public async Task<IActionResult> AcceptInvite(int inviteId)
    {
        EventInvite? invite = _applicationDbContext.EventInvites
            .Include(invite => invite.Event)
            .FirstOrDefault(invite => invite.Id == inviteId);

        if (invite == null)
        {
            return BadRequest(new
            {
                Success = false,
                Data = (object?)null,
                Error = "Invite not found."
            });
        }

        if (invite.Expires < DateTime.UtcNow)
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

        if (User.Identity?.IsAuthenticated != true)
        {
            return Unauthorized(new
            {
                Success = false,
                Data = (object?)null,
                Error = "You must be authenticated to accept an invite."
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