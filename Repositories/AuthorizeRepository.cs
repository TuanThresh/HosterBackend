using HosterBackend.Data;
using HosterBackend.Interfaces;
using HosterBackend.Data.Entities;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using HosterBackend.Dtos;
using HosterBackend.Helpers;
namespace HosterBackend.Repositories;

public class AuthorizeRepository(DataContext context,IMapper mapper) : IAuthorizeRepository
{

    public async Task AuthorizeEmployee(int employeeId, int roleId)
    {
        context.Authorizes.Add(new Authorize{
            EmployeeId = employeeId,
            RoleId = roleId
        });
        
        await context.SaveChangesAsync();
    }

    public async Task DeleteAuthorize(Authorize authorize)
    {
        context.Authorizes.Remove(authorize);

        await context.SaveChangesAsync();
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

    public async Task<PagedList<AuthorizeDto>> GetAuthorizeDtos(PagedListParams pagedListParams)
    {
        Console.WriteLine(pagedListParams.PageSize);

        var authorizes = context.Authorizes.ProjectTo<AuthorizeDto>(mapper.ConfigurationProvider);

        return await PagedList<AuthorizeDto>.GetPagedList(authorizes,pagedListParams.PageSize,pagedListParams.CurrentPage);
    }
    public async Task<Authorize> GetAuthorize(int employeeId,int roleId)
    {
        return await context.Authorizes.FirstOrDefaultAsync(x => x.EmployeeId == employeeId && x.RoleId == roleId) ?? throw new Exception("Không tìm thấy phân quyền");
    }

    public async Task<bool> SaveAllAsync()
    {
        return await context.SaveChangesAsync() > 0;
    }
}