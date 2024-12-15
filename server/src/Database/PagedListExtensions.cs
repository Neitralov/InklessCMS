namespace Database;

public static class PagedListExtensions
{
    public static async Task<PagedList<T>> ToPagedList<T>(
        this IQueryable<T> source, 
        PageOptions pageOptions, 
        CancellationToken cancellationToken)
    {
        var count = await source.CountAsync(cancellationToken);
        var items = await source
            .Skip((pageOptions.PageNumber - 1) * pageOptions.PageSize)
            .Take(pageOptions.PageSize)
            .ToListAsync(cancellationToken);
        
        return new PagedList<T>(items, count);
    }
}