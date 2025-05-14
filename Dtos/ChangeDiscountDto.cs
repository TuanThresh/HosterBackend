namespace HosterBackend.Dtos;

public class ChangeDiscountDto
{
    public int Percentage { get; set; }
    public int? CustomerTypeId { get; set; }
    public required string DiscountCode { get; set; }
    public string Description { get; set; } = "";
    public DateTime ExpiredAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}