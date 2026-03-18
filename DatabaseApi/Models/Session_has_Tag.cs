namespace DatabaseApi.Models
{
    public class Session_has_Tag
    {
        public int oSession { get; set; }
        public string idtag { get; set; }

        // Navigation properties
        public Session Session { get; set; }
        public Tag Tag { get; set; }
    }
}
