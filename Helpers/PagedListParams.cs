namespace HosterBackend.Helpers;

public class PagedListParams
{
    private const int MaxPageSize = 50;
    private int page_size = 10;
    public int CurrentPage { get; set; } = 1;
    public int PageSize { get => page_size; set => page_size = value > MaxPageSize ? MaxPageSize : value; }
}