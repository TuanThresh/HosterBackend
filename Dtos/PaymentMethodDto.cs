using HosterBackend.Helpers;

namespace HosterBackend.Dtos;

public class PaymentMethodDto
{
    public int Id { get; set; }
    public required string PaymentMethodName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}