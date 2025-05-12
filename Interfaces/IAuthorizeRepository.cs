using HosterBackend.Data.Entities;
using HosterBackend.Dtos;

namespace HosterBackend.Interfaces;

public interface IAuthorizeRepository
{
    public Task AuthorizeEmployee(int employeeId, int roleId);
    public Task<IEnumerable<Authorize>> GetAuthorizes(int employeeId,int roleId = -1);
    public Task DeleteAuthorize(Authorize authorize);
    public Task<IEnumerable<AuthorizeDto>> GetAuthorizeDtos();
    public Task<Authorize> GetAuthorize(int employeeId,int roleId);
    public Task<bool> SaveAllAsync();
}