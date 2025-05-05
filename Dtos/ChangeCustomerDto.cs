namespace HosterBackend.Dtos;

public class ChangeCustomerDto
{
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public byte[]? PasswordHash { get; set; }
    public byte[]? PasswordSalt { get; set; }
    public required string Address { get; set; }
    public required string PhoneNumber { get; set; }
    public int ContractTotal { get; set; } = 0;
    public int CustomerTypeId { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}