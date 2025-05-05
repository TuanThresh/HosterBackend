namespace HosterBackend.Dtos;

public class ChangePaymentMethodDto
{
    public required string PaymentMethodName { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}