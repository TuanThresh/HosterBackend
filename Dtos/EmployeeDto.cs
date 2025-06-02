using HosterBackend.Data.Enums;
using HosterBackend.Helpers;

namespace HosterBackend.Dtos;

public class EmployeeDto 
{
    public int Id { get; set; }
    public required string Email { get; set; }
    public required string Status { get; set; }
    public required string Name { get; set; }
    public required string Address { get; set; }
    public required string PhoneNumber { get; set; }
    public List<string> HasRoles { get; set; } = [];
}