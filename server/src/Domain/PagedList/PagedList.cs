namespace Domain.PagedList;

public sealed class PagedList<T> : List<T>
{
    public int TotalCount { get; private set; }

    public PagedList(List<T> items, int count)
    {
        TotalCount = count;
        AddRange(items);
    }
}
