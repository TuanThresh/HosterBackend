using HosterBackend.Data.Enums;

namespace HosterBackend.Dtos;

public class ChangeEmployeeDto
{
    public string? Name { get; set; }
    public string? Address { get; set; }
    public string? PhoneNumber { get; set; }
    public required string Email { get; set; }
    // public string? Password { get; set; }
    public byte[]? PasswordHash { get; set; }
    public byte[]? PasswordSalt { get; set; }
    public EmployeeStatusEnum Status { get; set; } = EmployeeStatusEnum.ChoXacThuc;
    // public List<int> Roles { get; set; } = [];
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}