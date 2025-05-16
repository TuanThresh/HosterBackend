namespace HosterBackend.Dtos;

public class ChangeCustomerDto
{
    public string? Name { get; set; }
    public string? Address { get; set; }
    public string? PhoneNumber { get; set; }
    public int? CustomerTypeId { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}