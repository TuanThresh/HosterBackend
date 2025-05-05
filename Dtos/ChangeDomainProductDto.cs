using HosterBackend.Data.Enums;

namespace HosterBackend.Dtos;

public class ChangeDomainProductDto
{
    public required string DomainName { get; set; }
    public int PriceStart { get; set; }
    public DomainTypeEnum DomainType { get; set; }
    public int Price { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}