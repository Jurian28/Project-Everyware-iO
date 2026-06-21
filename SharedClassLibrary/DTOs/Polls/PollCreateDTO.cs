using System.ComponentModel.DataAnnotations;

namespace SharedClassLibrary.DTOs.Polls;

public class PollCreateDTO
{
    [Required(ErrorMessage = "Title is required.")]
    public required string Title { get; set; }

    public string? Description { get; set; }

    [Required(ErrorMessage = "IdSession is required.")]
    public int IdSession { get; set; }

    [Required(ErrorMessage = "At least two answers are required.")]
    [MinLength(2, ErrorMessage = "At least two answers are required.")]
    public List<string> Answers { get; set; } = new();
}
