namespace HosterBackend.Dtos;

public class EmployeeAuthDto
{
    public required string Name { get; set; }
    public required string Token { get; set; }
    public List<string> HasRoles { get; set; } = [];
}