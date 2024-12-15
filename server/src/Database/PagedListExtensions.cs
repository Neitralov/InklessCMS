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
            .Skip((pageOptions.Page - 1) * pageOptions.Size)
            .Take(pageOptions.Size)
            .ToListAsync(cancellationToken);
        
        return new PagedList<T>(items, count);
    }
}