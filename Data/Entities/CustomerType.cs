namespace HosterBackend.Data.Entities;

public class CustomerType
{
    public int Id { get; set; }
    public required string TypeName { get; set; }
    public string Description { get; set; } = "";
    public List<Customer> HasCustomers { get; set; } = [];
    public Discount Discount { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}