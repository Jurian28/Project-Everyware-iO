namespace Back_office.DTOs;

public class InviteData
{
    public required int Invite { get; set; }
}

public class CreateInviteDto
{
    public required bool Success { get; set; }
    public required InviteData? Data { get; set; }
    public required string? Error { get; set; }
}
