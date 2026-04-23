namespace WebApp.Models.Dtos;

public class AcceptInviteDto
{
    public required bool Success { get; set; }
    public required object? Data { get; set; }
    public required string? Error { get; set; }
}
