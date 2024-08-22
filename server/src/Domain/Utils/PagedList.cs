namespace Domain.Utils;

public class PagedList<T> : List<T>
{
    public int TotalCount { get; private set; }

    public PagedList(List<T> items, int count)
    {
        TotalCount = count;
        AddRange(items);
    }
}

public static class PagedList
{
    public static Task<PagedList<T>> ToPagedList<T>(this IQueryable<T> source, int pageNumber, int pageSize)
    {
        var count = source.Count();
        var items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

        return Task.FromResult(new PagedList<T>(items, count));
    }
}