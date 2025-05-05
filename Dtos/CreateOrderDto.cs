namespace HosterBackend.Dtos;

public class CreateOrderDto
{
    public int DomainProductId { get; set; }
    public int? TotalPrice { get; set; }
    public int PaymentMethodId { get; set; }
    public int DiscountId { get; set; }
    public int CustomerId { get; set; }
    public int DurationByMonth { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}