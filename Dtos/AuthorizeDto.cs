namespace HosterBackend.Dtos;

public class AuthorizeDto
{
    public int Id { get; set; }
    public EmployeeDto Employee { get; set; } = null!;
    public RoleDto Role { get; set; } = null!;
}