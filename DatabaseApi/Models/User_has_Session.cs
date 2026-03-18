namespace DatabaseApi.Models
{
    public class User_has_Session
    {
        public int User_idUser { get; set; }
        public int oSession { get; set; }
        public bool inWaitingList { get; set; }
        public DateTime joinedDate { get; set; }

        // Navigation properties
        public User User { get; set; }
        public Session Session { get; set; }
    }
}
