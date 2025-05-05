namespace HosterBackend.Dtos;

public class ChangeEmployeeDto
{
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public byte[]? PasswordHash { get; set; }
    public byte[]? PasswordSalt { get; set; }
    public int Status { get; set; } = 2;
    public List<int> Roles { get; set; } = [];
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}