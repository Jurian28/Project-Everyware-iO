using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SharedClassLibrary.DTOs.Tags;

/// <summary>
/// Represents the input data for creating a Tag.
/// </summary>
public class TagInsertDTO
{
    [Required(ErrorMessage = "IdEvent is required.")]
    public int IdEvent { get; set; }

    [Required(ErrorMessage = "Title is required.")]
    public required string Title { get; set; }

    public string? ColorHex { get; set; }
}
