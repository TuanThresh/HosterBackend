using HosterBackend.Data.Enums;

namespace HosterBackend.Data.Entities;

public class Order
{
    public int Id { get; set; }
    public int DomainProductId { get; set; }
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
    public DomainProduct DomainProduct { get; set; } = null!;
    public int DiscountId { get; set; }
    public Discount Discount { get; set; } = null!;
    public int TotalPrice { get; set; }
    public int PaymentMethodId { get; set; }
    public PaymentMethod PaymentMethod { get; set; } = null!;
    public OrderStatusEnum Status { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}