namespace DatabaseApi.Models
{
    public class Speaker
    {
        public int idSpeaker { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }

        // Navigation properties
        public ICollection<Session> Sessions { get; set; } = new List<Session>();
    }
}
