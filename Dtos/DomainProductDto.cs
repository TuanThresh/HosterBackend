using HosterBackend.Data.Entities;
using HosterBackend.Data.Enums;

namespace HosterBackend.Dtos;

public class DomainProductDto
{
    public int Id { get; set; }
    public required string DomainName { get; set; }
    public required string DomainType { get; set; }
    public int Price { get; set; }
    // public List<RegisteredDomainDto> RegisteredDomains { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}