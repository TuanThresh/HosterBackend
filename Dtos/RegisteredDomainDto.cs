namespace HosterBackend.Dtos;

public class RegisteredDomainDto
{
    public int Id { get; set; }

    public required string FullDomainName { get; set; }
    public int DomainAccountId { get; set; }
    public DomainAccountDto DomainAccount { get; set; } = null!;
    public int DomainProductId { get; set; }
    public DomainProductDto DomainProduct { get; set; } = null!;
    public DateTime RegisteredAt { get; set; } = DateTime.Now;
    public DateTime ExpiredAt { get; set; }
    public int OrderId { get; set; }
    public OrderDto Order { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}