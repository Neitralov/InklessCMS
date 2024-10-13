namespace Database;

public static class PagedListExtensions
{
    public static async Task<PagedList<T>> ToPagedList<T>(this IQueryable<T> source, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var count = await source.CountAsync(cancellationToken);
        var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        
        return new PagedList<T>(items, count);
    }
}