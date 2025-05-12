using HosterBackend.Data.Enums;

namespace HosterBackend.Dtos;

public class AuthorizeEmployeeDto
{
    public int EmployeeId { get; set; }

    public int RoleId { get; set; }
}