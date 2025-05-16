namespace HosterBackend.Dtos;

public class ResetPasswordDto
{
    public required string Email { get; set; }
    public required string Token { get; set; } // mã đổi mật khẩu
    public required string NewPassword { get; set; }
    public required string ConfirmPassword { get; set; }
}