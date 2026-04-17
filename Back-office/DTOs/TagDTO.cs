namespace Back_office.DTOs
{
    public class TagDTO
    {
        public int IdTag { get; set; }
        public int IdEvent { get; set; }
        public string Title { get; set; } = string.Empty;


        public string? ColorHex { get; set; }
    }
}
