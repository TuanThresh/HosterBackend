namespace HosterBackend.Dtos;

public class ChangeCustomerTypeDto
{
    public required string TypeName { get; set; }
    public string Description { get; set; } = "";
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}