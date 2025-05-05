namespace HosterBackend.Data.Entities;

public class Discount
{
    public int Id { get; set; }
    public int Percentage { get; set; }
    public int CustomerTypeId { get; set; }
    public CustomerType CustomerType { get; set; } = null!;
    public List<Order> Orders { get; set; } = [];
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}