using HosterBackend.Data.Entities;
using HosterBackend.Helpers;

namespace HosterBackend.Dtos;

public class NewDto
{
    public int Id { get; set; }
    public required string Category { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
    public required string ImageUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}