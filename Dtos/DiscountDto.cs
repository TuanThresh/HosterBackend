using HosterBackend.Helpers;

namespace HosterBackend.Dtos;

public class DiscountDto
{
    public int Id { get; set; }
    public int Percentage { get; set; }
    public CustomerTypeDto CustomerType { get; set; } = null!;
    public List<OrderDto> Orders { get; set; } = [];
    public required string DiscountCode { get; set; }
    public string Description { get; set; } = "";
    public required string ExpiredAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}