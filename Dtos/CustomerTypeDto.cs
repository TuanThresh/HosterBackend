namespace HosterBackend.Dtos;

public class CustomerTypeDto
{
    public int Id { get; set; }
    public required string TypeName { get; set; }
    public string Description { get; set; } = "";
    public List<CustomerDto> HasCustomers { get; set; } = [];
    public DiscountDto Discount { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}