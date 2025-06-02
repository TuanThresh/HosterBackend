using HosterBackend.Helpers;

namespace HosterBackend.Dtos;

public class CustomerDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Address { get; set; }
    public required string PhoneNumber { get; set; }
    public int ContractTotal { get; set; }
    public required string HasType { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}