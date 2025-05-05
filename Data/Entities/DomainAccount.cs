namespace HosterBackend.Data.Entities;

public class DomainAccount
{
    public int Id { get; set; }
    public int DomainProductId { get; set; }
    public required string RegisterPanel { get; set; }
    public required string Username { get; set; }
    public byte[] PasswordHash { get; set; } = [];
    public byte[] PasswordSalt { get; set; } = [];
    public DomainProduct BelongsTo { get; set; } = null!;
    public DateTime ExpiredAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}