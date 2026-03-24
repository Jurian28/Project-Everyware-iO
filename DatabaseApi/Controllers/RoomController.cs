using DatabaseApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatabaseApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RoomController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RoomController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<Room>> GetAllRooms()
        {
            List<Room> rooms = await _context.Rooms.ToListAsync();
            return Ok(rooms);
        }

        [HttpGet("{roomId}")]
        public async Task<ActionResult<Room>> GetRoom(int roomId)
        {
            var room = await _context.Rooms.FindAsync(roomId);
            if (room == null) return NotFound();
            return Ok(room);
        }

        [HttpPost]
        public async Task<ActionResult<Room>> InsertRoom([FromBody] Room room)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                _context.Rooms.Add(room);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetRoom), new { id = room.IdRoom }, room);
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("UNIQUE") == true)
            {
                return Conflict("A room with that label already exists for this event");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Room>> UpdateRoom(int id, [FromBody] Room room)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existing = await _context.Rooms.FindAsync(id);
            if (existing == null) return NotFound();

            try
            {
                _context.Entry(existing).CurrentValues.SetValues(room);
                await _context.SaveChangesAsync();
                return Ok(existing);
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("UNIQUE") == true)
            {
                return Conflict("A room with that label already exists for this event");
            }
        }

        [HttpDelete("{buildingId}/{roomNumber}")]
        public async Task<IActionResult> DeleteRoom(int buildingId, int roomNumber)
        {
            var room = await _context.Rooms.FindAsync(buildingId, roomNumber);
            if (room == null) return NotFound();

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}