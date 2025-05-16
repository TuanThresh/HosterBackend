namespace HosterBackend.Data.Entities;

public class PasswordResetToken
{
    public int Id { get; set; }
    public required string Email { get; set; }
    public string Token { get; set; } = Guid.NewGuid().ToString();
    public DateTime ExpiredAt { get; set; }
    public bool IsUsed { get; set; } = false;
}