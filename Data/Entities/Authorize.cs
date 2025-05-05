

namespace HosterBackend.Data.Entities;

public class Authorize
{
    public int Id { get; set; }
    public Employee Employee { get; set; } = null!;
    public int EmployeeId { get; set; }
    public Role Role { get; set; } = null!;
    public int RoleId { get; set; }
}