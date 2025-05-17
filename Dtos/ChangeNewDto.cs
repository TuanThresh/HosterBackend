using HosterBackend.Data.Enums;

namespace HosterBackend.Dtos;

public class ChangeNewDto
{
    public int CategoryId { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
    public IFormFile Image { get; set; } = null!;
    public string? FileName { get; set; }
    public string? FilePath { get; set; }
    public string? ImageUrl { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}