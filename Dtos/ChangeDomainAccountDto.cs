namespace HosterBackend.Dtos;

public class ChangeDomainAccountDto 
{
    public required string RegisterPanel { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }
    public byte[]? PasswordHash { get; set; }
    public byte[]? PasswordSalt { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}