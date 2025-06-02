using HosterBackend.Data.Entities;
using HosterBackend.Helpers;

namespace HosterBackend.Dtos;

public class OrderDto
{
    public int Id { get; set; }
    public int DomainProductId { get; set; }
    public DomainProductDto DomainProduct { get; set; } = null!;
    public CustomerDto Customer { get; set; } = null!;
    public PaymentMethodDto PaymentMethod { get; set; } = null!;
    public int DurationByMonth { get; set; }
    public required string DomainFirstPart { get; set; }

    public int TotalPrice { get; set; }
    public required string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}