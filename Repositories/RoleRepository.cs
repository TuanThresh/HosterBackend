using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using HosterBackend.Data;
using HosterBackend.Data.Entities;
using HosterBackend.Dtos;
using HosterBackend.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HosterBackend.Repositories;

public class RoleRepository(DataContext context,IMapper mapper) : Repository<Role>(context,mapper),IRoleRepository
{
    // public async Task<IEnumerable<Role>> GetRoles()
    // {
    //     return await context.Roles.Include(x => x.GivenEmployees).ToListAsync();
    // }
    // public async Task<IEnumerable<RoleDto>> GetRoleDtos()
    // {
    //     return await context.Roles
    //         .Include(x => x.GivenEmployees)
    //         .ThenInclude(x => x.Employee)
    //         .ProjectTo<RoleDto>(mapper.ConfigurationProvider).ToListAsync();
    // }
    // public async Task CreateRole(ChangeRoleDto createRoleDto)
    // {
    //     if(await context.Roles.AnyAsync(x => x.RoleName.Equals(createRoleDto.RoleName)))
    //     {
    //         throw new Exception("Đã có vai trò với tên " + createRoleDto.RoleName);
    //     }

    //     var role = mapper.Map<Role>(createRoleDto);

    //     context.Roles.Add(role);
    // }

    // public async Task UpdateRole(ChangeRoleDto updateRole,Role role)
    // {
    //     if(await context.Roles.AnyAsync(x => x.RoleName.Equals(updateRole.RoleName)))
    //     {
    //         throw new Exception("Đã có vai trò với tên " + updateRole.RoleName);
    //     }
        
    //     mapper.Map(updateRole,role);
        
    // }

    // public void DeleteRole(Role role)
    // {
    //     context.Roles.Remove(role);
    // }

    // public async Task<Role> GetRoleById(int id)
    // {
    //     return await context.Roles
    //                     .Where(x => x.Id == id)
    //                     .Include(x => x.GivenEmployees)
    //                     .ThenInclude(x => x.Employee)
    //                     .FirstOrDefaultAsync()
    //                     ?? throw new Exception("Không tìm thấy vai trò với id " + id);
    // }
    // public async Task<RoleDto> GetRoleDtoById(int id)
    // {
    //     return await context.Roles
    //                     .Where(x => x.Id == id)
    //                     .Include(x => x.GivenEmployees)
    //                     .ThenInclude(x => x.Employee)
    //                     .ProjectTo<RoleDto>(mapper.ConfigurationProvider)
    //                     .FirstOrDefaultAsync()
    //                     ?? throw new Exception("Không tìm thấy vai trò với id " + id);
    // }

    // public async Task<bool> SaveAllAsync()
    // {
    //     return await context.SaveChangesAsync() > 0;
    // }
}