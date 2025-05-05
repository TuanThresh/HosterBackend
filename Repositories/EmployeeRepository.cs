using AutoMapper;
using AutoMapper.QueryableExtensions;
using HosterBackend.Data;
using HosterBackend.Data.Entities;
using HosterBackend.Dtos;
using HosterBackend.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace HosterBackend.Repositories;

public class EmployeeRepository(DataContext context, IMapper mapper) : Repository<Employee>(context,mapper),IEmployeeRepository
{
    private readonly DataContext _context = context;

    public async Task<Employee?> GetEmployeeByEmailOrName(string email, string name)
    {
        var query = _context.Employees.Include(x => x.HasRoles).ThenInclude(x => x.Role).AsQueryable();

        var employee = await query.SingleOrDefaultAsync(x => x.Name == name);

        if(employee != null) return employee;

        return await query.SingleOrDefaultAsync(x => x.Email == email);
    }

}