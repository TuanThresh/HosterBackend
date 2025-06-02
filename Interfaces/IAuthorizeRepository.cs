using HosterBackend.Data.Entities;
using HosterBackend.Dtos;
using HosterBackend.Helpers;

namespace HosterBackend.Interfaces;

public interface IAuthorizeRepository
{
    public Task AuthorizeEmployee(int employeeId, int roleId);
    public Task<IEnumerable<Authorize>> GetAuthorizes(int employeeId,int roleId = -1);
    public Task DeleteAuthorize(Authorize authorize);
    public Task<PagedList<AuthorizeDto>> GetAuthorizeDtos(PagedListParams pagedListParams);
    public Task<Authorize> GetAuthorize(int employeeId,int roleId);
    public Task<bool> SaveAllAsync();
}