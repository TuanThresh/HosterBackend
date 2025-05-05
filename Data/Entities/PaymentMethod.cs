namespace HosterBackend.Data.Entities;

public class PaymentMethod
{
    public int Id { get; set; }
    public required string PaymentMethodName { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}