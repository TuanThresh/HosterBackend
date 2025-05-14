using HosterBackend.Data.Entities;

namespace HosterBackend.Dtos;

public class CreateOrderDto
{
    public int DomainProductId { get; set; }
    public required string DomainFirstPart { get; set; }
    public int? TotalPrice { get; set; }
    public int PaymentMethodId { get; set; }
    public string? DiscountCode { get; set; }
    public int CustomerId { get; set; }
    public int DurationByMonth { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}