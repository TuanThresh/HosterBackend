using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using HosterBackend.Data;
using HosterBackend.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HosterBackend.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly DbContext _context;
    private readonly DbSet<T> _dbSet;
    private readonly IMapper _mapper;
    public Repository(DataContext context,IMapper mapper)
    {
        _context = context;
        _dbSet = _context.Set<T>();
        _mapper = mapper;

    }

    public async Task<T> AddAsync<TDto>(TDto entity,params string[] conditions) where TDto : class
    {
        var mappedEntity = _mapper.Map<T>(entity);

        if(conditions.Length > 0)
        {
            foreach (var condition in conditions)
            {
                var entityProperty = GetEntityProperty<string>(mappedEntity,condition);

                if(await _dbSet.AnyAsync(x => EF.Property<string>(x,condition).ToLower() == entityProperty.ToLower()))
                {
                    throw new Exception($"Đã có {typeof(T).Name} với {condition} là " + entityProperty);
                }
            }
        }

        _dbSet.Add(mappedEntity);

        await _context.SaveChangesAsync();

        return mappedEntity;
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await GetByIdAsync(id);

        _dbSet.Remove(entity);

        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes)
    {
        var query = _dbSet.AsQueryable();

        query = ExtendEntity(query,includes);

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<TDto>> GetAllDtoAsync<TDto>(params Expression<Func<T, object>>[] includes) where TDto : class
    {
        var query = _dbSet.AsQueryable();

        query = ExtendEntity(query,includes);

        return await query
                    .ProjectTo<TDto>(_mapper.ConfigurationProvider)
                    .ToListAsync();
    }

    public async Task<T> GetByIdAsync(int id, params Expression<Func<T, object>>[] includes)
    {
        var query = _dbSet.AsQueryable();

        query = ExtendEntity(query,includes);

        return await query.FirstOrDefaultAsync(x => EF.Property<int>(x, "Id")  == id)
                            ?? throw new Exception($"Không tìm thấy id của {typeof(T).Name}");
    }

    public async Task<TDto> GetDtoByIdAsync<TDto>(int id, params Expression<Func<T, object>>[] includes) where TDto : class
    {
        var query = _dbSet.AsQueryable();

        query = ExtendEntity(query,includes);

        return await query.Where(x => EF.Property<int>(x, "Id")  == id)
                    .ProjectTo<TDto>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync()
                    ?? throw new Exception($"Không tìm thấy id của {typeof(T).Name}");
                    
                            
    }

    public async Task UpdateAsync<TDto>(int id,TDto entity,params string[] conditions) where TDto : class
    {
        var existingEntity = await GetByIdAsync(id);

        var mappedEntity = _mapper.Map<T>(entity);

        if(conditions.Length > 0)
        {
            foreach (var condition in conditions)
            {
                var entityProperty = GetEntityProperty<string>(mappedEntity,condition);

                if(await _dbSet.AnyAsync(x => EF.Property<string>(x,condition).ToLower() == entityProperty.ToLower()))
                {
                    throw new Exception($"Đã có {typeof(T).Name} với {condition} là " + entityProperty);
                }
            }
        }

        _mapper.Map(entity,existingEntity);

        _dbSet.Update(existingEntity);

        await _context.SaveChangesAsync();
    }
    private static IQueryable<T> ExtendEntity(IQueryable<T> query,params Expression<Func<T, object>>[]  includes)
    {
        if(includes.Length == 0) return query;
        
        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        return query;
    }
         private static TProp GetEntityProperty<TProp>(T entity,string property)
    {
        var entityProperty = typeof(T).GetProperty(property)?.GetValue(entity) ?? throw new Exception("Không tìm thấy trường id cho thực thể này");

        return (TProp)entityProperty;
    }

    public async Task<T> GetByPropertyAsync(Expression<Func<T, bool>> predicate,params Expression<Func<T, object>>[] includes)
    {
        var query = ExtendEntity(_dbSet,includes);
        
        return await query.FirstOrDefaultAsync(predicate) ?? throw new Exception($"Không tìm thấy {typeof(T).Name}");
    }
    public async Task<IEnumerable<TDto>> GetAllDtoByPropertyAsync<TDto>(Expression<Func<T, bool>> predicate,params Expression<Func<T, object>>[] includes) where TDto : class
    {
        var query = ExtendEntity(_dbSet,includes);
        
        return await query.Where(predicate).ProjectTo<TDto>(_mapper.ConfigurationProvider).ToListAsync() ?? throw new Exception($"Không tìm thấy {typeof(T).Name}");
    }

    public async Task<TDto> GetDtoByPropertyAsync<TDto>(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes) where TDto : class
    {
        var query = ExtendEntity(_dbSet,includes);

        return await query.Where(predicate).ProjectTo<TDto>(_mapper.ConfigurationProvider).FirstOrDefaultAsync() ?? throw new Exception($"Không tìm thấy {typeof(T).Name}");
    }
    public async Task<bool> CheckExistsAsync(Expression<Func<T,bool>> predicate)
    {
        return await _dbSet.AnyAsync(predicate);
    }
}