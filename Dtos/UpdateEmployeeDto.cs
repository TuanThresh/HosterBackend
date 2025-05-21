using HosterBackend.Data.Enums;

namespace HosterBackend.Dtos;

public class UpdateEmployeeDto
{
    public required string Name { get; set; }
    public required string Address { get; set; }
    public required string PhoneNumber { get; set; }
    public required string Email { get; set; }
    // public string? Password { get; set; }
    public EmployeeStatusEnum Status { get; set; } = EmployeeStatusEnum.ChoXacThuc;
    // public List<int> Roles { get; set; } = [];
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}