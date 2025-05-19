using HosterBackend.Data.Enums;

namespace HosterBackend.Dtos;

public class CreateEmployeeDto
{
    public required string Name { get; set; }
    public required string Address { get; set; }
    public required string PhoneNumber { get; set; }
    public required string Email { get; set; }
    public byte[]? PasswordHash { get; set; }
    public byte[]? PasswordSalt { get; set; }
    // public string? Password { get; set; }
    public EmployeeStatusEnum Status { get; set; } = EmployeeStatusEnum.ChoXacThuc;
    // public List<int> Roles { get; set; } = [];
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}