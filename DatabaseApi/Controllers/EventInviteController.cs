using DatabaseApi.Models;
using DatabaseApi.Models.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatabaseApi.Controllers;

[ApiController]
[Route("api/invites")]
public class EventInviteController(ApplicationDbContext applicationDbContext) : Controller
{
    private readonly ApplicationDbContext _applicationDbContext = applicationDbContext;

    [HttpPost("create")]
    public async Task<IActionResult> CreateInvite([FromBody] EventInviteCreateDto eventInviteCreateDto)
    {
        Event? @event = await _applicationDbContext.Events.FindAsync(eventInviteCreateDto.EventId);

        if (@event == null)
        {
            return BadRequest("Event not found");
        }

        if (eventInviteCreateDto.Expires < DateTime.UtcNow)
        {
            return BadRequest("Expiration date must be in the future.");
        }

        EventInvite invite = new()
        {
            Event = @event,
            Expires = eventInviteCreateDto.Expires
        };
        @event.Invites.Add(invite);
        await _applicationDbContext.SaveChangesAsync();

        return StatusCode(201, new
        {
            Invite = invite.Id
        });
    }

    [HttpPost("accept")]
    public async Task<IActionResult> AcceptInvite([FromBody] int inviteId)
    {
        EventInvite? invite = await _applicationDbContext.EventInvites.FindAsync(inviteId);

        if (invite == null)
        {
            return BadRequest("Invite not found.");
        }

        if (invite.Expires < DateTime.UtcNow)
        {
            invite.Event.Invites.Remove(invite);
            await _applicationDbContext.SaveChangesAsync();

            return BadRequest("Invite has expired.");
        }

        User? user = await _applicationDbContext.Users.FindAsync(User.Claims.FirstOrDefault(x => x.Type == "id")?.Value);

        if (user is null)
        {
            return BadRequest("User not found.");
        }

        user.Events.Add(invite.Event);
        await _applicationDbContext.SaveChangesAsync();

        return Ok();
    }
}