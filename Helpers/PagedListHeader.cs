namespace HosterBackend.Helpers;

public class PagedListHeader<T>
{
    public PagedListHeader(PagedList<T> data)
    {
        TotalCount = data.TotalCount;
        TotalPages = data.TotalPages;
        CurrentPage = data.CurrentPage;
        PageSize = data.PageSize;
    }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
}