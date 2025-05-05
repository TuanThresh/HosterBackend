using HosterBackend.Data.Entities;

namespace HosterBackend.Interfaces;

public interface IAuthorizeRepository
{
    public void AuthorizeEmployee(int employeeId, int roleId);
    public Task<IEnumerable<Authorize>> GetAuthorizes(int employeeId,int roleId = -1);
    public void DeleteAuthorize(Authorize authorize);

    public Task<bool> SaveAllAsync();
}