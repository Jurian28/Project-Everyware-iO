namespace Back_office.DTO
{
    public class TagDto
    {
        public int IdEvent { get; set; }
        public string Title { get; set; } = string.Empty;

        public string? OldTitle { get; set; }

        public string? ColorHex { get; set; }
    }
}
