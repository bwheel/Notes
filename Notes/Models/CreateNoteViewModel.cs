using System.ComponentModel.DataAnnotations;

namespace Notes.Models;

public class CreateNoteViewModel
{
    [Required(AllowEmptyStrings = false)]
    public required string Content { get; set; }

    public string? ErrorMessage { get; set; }
}