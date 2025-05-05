using HosterBackend.Data.Entities;

namespace HosterBackend.Dtos;

public class OrderDto
{
    public int Id { get; set; }
    public int DomainProductId { get; set; }
    public DomainProductDto DomainProduct { get; set; } = null!;
    public CustomerDto Customer { get; set; } = null!;
    public PaymentMethodDto PaymentMethod { get; set; } = null!;
    public int TotalPrice { get; set; }
    public required string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}