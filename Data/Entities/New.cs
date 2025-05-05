namespace HosterBackend.Data.Entities;

public class New
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
    public required string ImageUrl { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}