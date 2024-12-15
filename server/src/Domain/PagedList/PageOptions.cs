namespace Domain.PagedList;

public struct PageOptions
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public PageOptions() { }
}