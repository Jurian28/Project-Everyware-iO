using Azure;
using DatabaseApi.DTOs.Rooms;
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
        public async Task<ActionResult<IEnumerable<RoomDTOs>>> GetAllRooms([FromQuery] int? eventId)
        {
            IQueryable<Room> query = _context.Rooms;

            if (eventId.HasValue)
            {   
                query = query.Where(r => r.IdEvent == eventId.Value);
            }

            var rooms = await query.ToListAsync();
            return Ok(rooms.Select(RoomMapper.ToResponseDTO));
        }

        [HttpPost]
        public async Task<ActionResult<RoomDTOs>> InsertRoom([FromBody] RoomInsertDTO roomDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            bool exists = await _context.Rooms.AnyAsync(r =>
                r.RoomLabel == roomDTO.RoomLabel &&
                r.IdEvent == roomDTO.IdEvent);

            if (exists)
                return Conflict("A room with that label already exists for this event");

            try
            {
                Room room = RoomMapper.ToEntity(roomDTO);
                _context.Rooms.Add(room);
                await _context.SaveChangesAsync();
                return StatusCode(201, RoomMapper.ToResponseDTO(room));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex}");

                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Room>> UpdateRoom(int id, [FromBody] RoomUpdateDTO roomDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var room = await _context.Rooms.FindAsync(id);
            if (room == null) return NotFound();

            try
            {
                RoomMapper.UpdateEntity(room, roomDTO);
                await _context.SaveChangesAsync();
                return Ok(room);
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("UNIQUE") == true)
            {
                return Conflict("A room with that label already exists for this event");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null) return NotFound();

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}