namespace Back_office.DTO
{
    public class TagDto
    {
        public int IdTag { get; set; }
        public int IdEvent { get; set; }
        public string Title { get; set; } = string.Empty;


        public string? ColorHex { get; set; }
    }
}
