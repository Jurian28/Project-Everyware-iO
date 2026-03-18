using Microsoft.AspNetCore.Identity;

namespace DatabaseApi.Models
{
    public class User : IdentityUser
    {
        // Navigation properties
        public ICollection<User_has_Session> Sessions { get; set; } = new List<User_has_Session>();
        public ICollection<Event> Events { get; set; } = new List<Event>();
    }
}
