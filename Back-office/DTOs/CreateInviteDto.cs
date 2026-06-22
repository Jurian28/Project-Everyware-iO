namespace Back_office.DTOs;

public class InviteData
{
    public required string Token { get; set; }
}

public class CreateInviteDto
{
    public required bool Success { get; set; }
    public required InviteData? Data { get; set; }
    public required string? Error { get; set; }
}
