namespace HosterBackend.Dtos;

public class ChangeCategoryDto
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public int MyProperty { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}