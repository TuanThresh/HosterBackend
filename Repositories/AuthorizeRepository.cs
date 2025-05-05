using HosterBackend.Data;
using HosterBackend.Interfaces;
using HosterBackend.Data.Entities;
using Microsoft.EntityFrameworkCore;
namespace HosterBackend.Repositories;

public class AuthorizeRepository(DataContext context) : IAuthorizeRepository
{

    public void AuthorizeEmployee(int employeeId, int roleId)
    {
        context.Authorizes.Add(new Authorize{
            EmployeeId = employeeId,
            RoleId = roleId
        });
        
        context.SaveChangesAsync();
    }

    public void DeleteAuthorize(Authorize authorize)
    {
        context.Authorizes.Remove(authorize);
    }

    public async Task<IEnumerable<Authorize>> GetAuthorizes(int employeeId, int roleId = -1)
    {
        IEnumerable<Authorize> authorizes = roleId switch
        {
            -1 => await context.Authorizes.Where(x => x.EmployeeId == employeeId).ToListAsync(),
            _ => await context.Authorizes.Where(x => x.EmployeeId == employeeId && x.RoleId == roleId).ToListAsync(),
        };
        return authorizes;
    }

    public async Task<bool> SaveAllAsync()
    {
        return await context.SaveChangesAsync() > 0;
    }
}