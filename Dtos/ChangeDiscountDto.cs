namespace HosterBackend.Dtos;

public class ChangeDiscountDto
{
    public int Percentage { get; set; }
    public int CustomerTypeId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}