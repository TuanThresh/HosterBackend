namespace HosterBackend.Dtos;

public class ChangeRoleDto
{
    public required string RoleName { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}