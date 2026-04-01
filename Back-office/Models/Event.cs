namespace Back_office.Models
{
    public class Event
    {
        public int IdEvent { get; set; }
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public string MainColorHex { get; set; }
        public string AccentColorHex { get; set; }
        public string LogoPath { get; set; }

        public ICollection<Room> Rooms { get; set; } = new List<Room>();
        public ICollection<Tag> Tags { get; set; } = new List<Tag>();
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
