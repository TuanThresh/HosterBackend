using HosterBackend.Data.Entities;
using HosterBackend.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace HosterBackend.Interfaces;

public interface IEmployeeRepository : IRepository<Employee>
{
    public Task<Employee?> GetEmployeeByEmailOrName(string email, string name);
}