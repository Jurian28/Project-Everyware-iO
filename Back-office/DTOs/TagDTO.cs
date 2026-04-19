namespace Back_office.DTOs
{
    public class TagDTO
    {
        public int IdTag { get; set; }
        public int IdEvent { get; set; }
        public string Title { get; set; } = "";


        public string? ColorHex { get; set; }
    }
}
