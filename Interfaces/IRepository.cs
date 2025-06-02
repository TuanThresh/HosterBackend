namespace HosterBackend.Interfaces;
using System.Linq.Expressions;
using HosterBackend.Helpers;

public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes);
    Task<T> GetByIdAsync(int id, params Expression<Func<T, object>>[] includes);
    Task<T?> GetByPropertyAsync(Expression<Func<T, bool>> predicate,params Expression<Func<T, object>>[] includes);
    Task<IEnumerable<T>> GetAllByPropertyAsync(Expression<Func<T, bool>> predicate,params Expression<Func<T, object>>[] includes);

    Task<PagedList<TDto>> GetAllDtoByPropertyAsync<TDto>(PagedListParams pagedListParams,Expression<Func<T, bool>> predicate,params Expression<Func<T, object>>[] includes) where TDto : class;
    Task<TDto> GetDtoByPropertyAsync<TDto>(Expression<Func<T, bool>> predicate,params Expression<Func<T, object>>[] includes) where TDto : class;
    Task<bool> CheckExistsAsync(Expression<Func<T,bool>> predicate);

    Task<T> AddAsync<TDto>(TDto entity,params string[] conditions) where TDto : class;
    Task UpdateAsync<TDto>(int id,TDto entity,params string[] conditions) where TDto : class;
    Task DeleteAsync(int id);

    Task<PagedList<TDto>> GetAllDtoAsync<TDto>(PagedListParams pagedListParams,params Expression<Func<T, object>>[] includes) where TDto : class;
    Task<TDto> GetDtoByIdAsync<TDto>(int id, params Expression<Func<T, object>>[] includes) where TDto : class;
}