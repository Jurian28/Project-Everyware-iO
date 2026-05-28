using Microsoft.AspNetCore.Identity;

namespace DatabaseApi.Models
{
    public class User : IdentityUser
    {
        public bool HasRequestedAccess { get; set; } = false;
        // Navigation properties
        public ICollection<User_has_Session> RegisteredSessions { get; set; } = new List<User_has_Session>();
        public ICollection<Event> Events { get; set; } = new List<Event>();
    }
}
