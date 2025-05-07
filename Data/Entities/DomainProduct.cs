using HosterBackend.Data.Enums;

namespace HosterBackend.Data.Entities;

public class DomainProduct
{
    public int Id { get; set; }
    public required string DomainName { get; set; }
    public int PriceStart { get; set; }
    public DomainTypeEnum DomainType { get; set; }
    public int Price { get; set; }
    public List<RegisteredDomain> RegisteredDomains { get; set; } = [];
    public List<Order> Orders { get; set; } = [];
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}