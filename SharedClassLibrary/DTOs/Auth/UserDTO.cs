using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SharedClassLibrary.DTOs.Auth;

public class UserDTO
{
    [Required]
    public required string Id { get; set; }
    [Required]
    public required string Email { get; set; }
}
