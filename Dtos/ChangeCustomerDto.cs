namespace HosterBackend.Dtos;

public class ChangeCustomerDto
{
    public required string Name { get; set; }
    public required string Address { get; set; }
    public required string PhoneNumber { get; set; }
    public int CustomerTypeId { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}