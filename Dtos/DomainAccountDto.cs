namespace HosterBackend.Dtos;

public class DomainAccountDto
{
    public int Id { get; set; }
    public required string RegisterPanel { get; set; }
    public required string Username { get; set; }
    // public byte[] PasswordHash { get; set; } = [];
    // public byte[] PasswordSalt { get; set; } = [];
    // public List<RegisteredDomainDto> RegisteredDomains { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}