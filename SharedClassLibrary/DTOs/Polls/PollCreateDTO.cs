using System.ComponentModel.DataAnnotations;

namespace SharedClassLibrary.DTOs.Polls;

public class PollCreateDTO
{
    [Required(ErrorMessage = "Title is required.")]
    public required string Title { get; set; }

    public string? Description { get; set; }

    [Required(ErrorMessage = "IdSession is required.")]
    public int IdSession { get; set; }

    [Required(ErrorMessage = "At least one answer is required.")]
    [MinLength(1, ErrorMessage = "At least one answer is required.")]
    public List<string> Answers { get; set; } = new();
}
