public class SessionDTO
{
    public int SessionId { get; set; }
    public string Title { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool Plenary { get; set; }
    public int? Capacity { get; set; }
    public int IdRoom { get; set; }
}