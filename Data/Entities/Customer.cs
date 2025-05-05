namespace HosterBackend.Data.Entities;

public class Customer
{
    public int Id { get; set; }
    public int CustomerTypeId { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Address { get; set; }
    public required string PhoneNumber { get; set; }
    public int ContractTotal { get; set; }
    public byte[] PasswordHash { get; set; } = [];
    public byte[] PasswordSalt { get; set; } = [];
    public CustomerType HasType { get; set; } = null!;
    public List<Order> Orders { get; set; } = [];
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}