using System.ComponentModel.DataAnnotations;

namespace Notes.Models;

public class EditNoteViewModel
{
    [Required(AllowEmptyStrings = false)]
    public required string Id { get; set; }

    [Required(AllowEmptyStrings = true)]
    public required string Content { get; set; }

    public string? ErrorMessage { get; set; }
}