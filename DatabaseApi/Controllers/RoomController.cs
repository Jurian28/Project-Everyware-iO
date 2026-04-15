using DatabaseApi.DTOs.Rooms;
using DatabaseApi.Hubs;
using DatabaseApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SharedClassLibrary.ApiResponse;
namespace DatabaseApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    /// <summary>
    /// Controller to handle all room input and output.
    /// </summary>
    public class RoomController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<RoomHub> _hubContext;
        public RoomController(ApplicationDbContext context, IHubContext<RoomHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }
        /// <summary>
        /// gets all rooms, if eventId is provided it gets all rooms for that event, otherwise it gets all rooms in the database
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomResponseDTO>>> GetAllRooms([FromQuery] int? eventId)
        {
            IQueryable<Room> query = _context.Rooms;

            if (eventId.HasValue)
            {   
                query = query.Where(r => r.IdEvent == eventId.Value);
            }

            var rooms = await query.ToListAsync();
            return Ok(ApiResponse<IEnumerable<RoomResponseDTO>>.Ok(rooms.Select(RoomMapper.ToResponseDTO)));
        }
        /// <summary>
        /// Inserts a new room into the database. The room data is provided in the request body as a RoomInsertDTO.
        /// </summary>

        [HttpPost]
        public async Task<ActionResult<ApiResponse<RoomResponseDTO>>> InsertRoom([FromBody] RoomInsertDTO roomDTO)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(e => e.Value.Errors.Count > 0)
                    .ToDictionary(
                        e => e.Key,
                        e => e.Value.Errors.Select(err => err.ErrorMessage).ToArray()
                    );

                return BadRequest(ApiResponse<object>.Fail("Validation failed", errors));
            }

            try
            {
                Room room = RoomMapper.ToEntity(roomDTO);
                _context.Rooms.Add(room);
                await _context.SaveChangesAsync();

                var response = RoomMapper.ToResponseDTO(room);
                await _hubContext.Clients.All.SendAsync("RoomsChanged");
                return StatusCode(201, ApiResponse<RoomResponseDTO>.Ok(response));
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx)
            {
                return sqlEx.Number switch
                {
                    547 => NotFound(ApiResponse<RoomResponseDTO>.Fail($"Event with ID {roomDTO.IdEvent} not found.")),
                    2601 => Conflict(ApiResponse<RoomResponseDTO>.Fail("A room with that label already exists for this event.")),
                    2627 => Conflict(ApiResponse<RoomResponseDTO>.Fail("A room with that label already exists for this event.")),
                    _ => StatusCode(500, ApiResponse<RoomResponseDTO>.Fail("An unexpected database error occurred."))
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex}");
                return StatusCode(500, ApiResponse<RoomResponseDTO>.Fail("An unexpected error occurred."));
            }
        }
        /// <summary>
        /// Updates an existing room in the database. The room ID is provided as a route parameter, and the updated room data is provided in the request body as a RoomUpdateDTO.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<Room>> UpdateRoom(int id, [FromBody] RoomUpdateDTO roomDTO)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(e => e.Value.Errors.Count > 0)
                    .ToDictionary(
                        e => e.Key,
                        e => e.Value.Errors.Select(err => err.ErrorMessage).ToArray()
                    );

                return BadRequest(ApiResponse<object>.Fail("Validation failed", errors));
            }


            var room = await _context.Rooms.FindAsync(id);
            if (room == null) return NotFound();

            try
            {
                RoomMapper.UpdateEntity(room, roomDTO);
                await _context.SaveChangesAsync();

                await _hubContext.Clients.All.SendAsync("RoomsChanged");
                return Ok(ApiResponse<RoomResponseDTO>.Ok(RoomMapper.ToResponseDTO(room)));
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx)
            {
                return sqlEx.Number switch
                {
                    _ => StatusCode(500, ApiResponse<RoomResponseDTO>.Fail("An unexpected error occurred."))
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex}");
                return StatusCode(500, ApiResponse<RoomResponseDTO>.Fail("An unexpected error occurred."));
            }
        }
        /// <summary>
        /// Deletes a room from the database. The room ID is provided as a route parameter. If the room has associated sessions, it cannot be deleted and a conflict response is returned.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null) return NotFound(ApiResponse<RoomResponseDTO>.Fail("Room not found."));
            try
            {
                _context.Rooms.Remove(room);
                await _context.SaveChangesAsync();
                return StatusCode(204, ApiResponse<RoomResponseDTO>.Ok(null));
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx)
            {
                return sqlEx.Number switch
                {
                    547 => Conflict(ApiResponse<RoomResponseDTO>.Fail("This room cannot be deleted because it still has Sessions.")),
                    _ => StatusCode(500, ApiResponse<RoomResponseDTO>.Fail("An unexpected error occurred."))
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex}");
                return StatusCode(500, ApiResponse<RoomResponseDTO>.Fail("An unexpected error occurred."));
            }
        }
    }
}