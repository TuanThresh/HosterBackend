namespace HosterBackend.Data.Entities;

public class Role
{
    public int Id { get; set; }
    public required string RoleName { get; set; }
    public List<Authorize> GivenEmployees { get; set; } = [];
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}