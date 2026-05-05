using System;
using System.Collections.Generic;
using System.Text;

namespace SharedClassLibrary.DTOs.Tags;

/// <summary>
/// Represents the input data for updating a Tag.
/// </summary>
public class TagUpdateDTO
{
    public required string Title { get; set; }
    public required string ColorHex { get; set; }
}
