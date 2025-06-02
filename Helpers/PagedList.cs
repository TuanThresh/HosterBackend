using Microsoft.EntityFrameworkCore;

namespace HosterBackend.Helpers;

public class PagedList<T> : List<T>
{
    public PagedList(IEnumerable<T> items, int count, int pageSize, int pageNumber)
    {
        TotalCount = count;
        PageSize = pageSize;
        CurrentPage = pageNumber;
        TotalPages = (int)Math.Ceiling((double)count / pageSize);
        AddRange(items);
    }

    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
    public int PageSize { get; set; }
    public int CurrentPage { get; set; }
    
    public static async Task<PagedList<T>> GetPagedList(IQueryable<T> source,int PageSize,int CurrentPage)
    {
        var count = await source.CountAsync();

        if (CurrentPage < 1) CurrentPage = 1;

        var items = await source.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToListAsync();

        return new PagedList<T>(items, count, PageSize, CurrentPage);
    } 
}