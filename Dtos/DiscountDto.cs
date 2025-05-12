namespace HosterBackend.Dtos;

public class DiscountDto
{
    public int Id { get; set; }
    public int Percentage { get; set; }
    // public CustomerTypeDto CustomerType { get; set; } = null!;
    // public List<OrderDto> Orders { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}