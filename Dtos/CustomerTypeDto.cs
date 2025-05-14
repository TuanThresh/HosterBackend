namespace HosterBackend.Dtos;

public class CustomerTypeDto
{
    public int Id { get; set; }
    public required string TypeName { get; set; }
    public string Description { get; set; } = "";
    public List<CustomerDto> HasCustomers { get; set; } = [];
    public List<DiscountDto> Discounts { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}