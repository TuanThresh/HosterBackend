namespace HosterBackend.Interfaces;
using System.Linq.Expressions;
public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes);
    Task<T> GetByIdAsync(int id, params Expression<Func<T, object>>[] includes);
    Task<T> GetByPropertyAsync(Expression<Func<T, bool>> predicate,params Expression<Func<T, object>>[] includes);
    Task<IEnumerable<TDto>> GetAllDtoByPropertyAsync<TDto>(Expression<Func<T, bool>> predicate,params Expression<Func<T, object>>[] includes) where TDto : class;
    Task<TDto> GetDtoByPropertyAsync<TDto>(Expression<Func<T, bool>> predicate,params Expression<Func<T, object>>[] includes) where TDto : class;

    Task<T> AddAsync<TDto>(TDto entity,params string[] conditions) where TDto : class;
    Task UpdateAsync<TDto>(int id,TDto entity,params string[] conditions) where TDto : class;
    Task DeleteAsync(int id);

    Task<IEnumerable<TDto>> GetAllDtoAsync<TDto>(params Expression<Func<T, object>>[] includes) where TDto : class;
    Task<TDto> GetDtoByIdAsync<TDto>(int id, params Expression<Func<T, object>>[] includes) where TDto : class;
}