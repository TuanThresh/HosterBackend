using HosterBackend.Data.Enums;

namespace HosterBackend.Data.Entities;

public class Employee
{
    public int Id { get; set; }
    public required string Email { get; set; }
    public EmployeeStatusEnum Status { get; set; } = EmployeeStatusEnum.ChoXacThuc;
    public required string Name { get; set; }
    public byte[] PasswordHash { get; set; } = [];
    public byte[] PasswordSalt { get; set; } = [];
    public List<Authorize> HasRoles { get; set; } = [];
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}