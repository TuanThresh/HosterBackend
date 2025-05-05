namespace HosterBackend.Dtos;

public class CustomerAuthDto
{
    public required string Name { get; set; }
    public required string Token { get; set; }
    public required string HasType { get; set; }
}