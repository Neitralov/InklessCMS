namespace Database.Repositories;

public class CollectionRepository(DatabaseContext database) : ICollectionRepository
{
    public async Task AddCollection(Collection newCollection)
    {
        await database.AddAsync(newCollection);
    }

    public async Task<ErrorOr<Collection>> FindCollectionById(string collectionId)
    {
        var collection = await database.Collections
            .Include(collection => collection.Articles)
            .SingleOrDefaultAsync(collection => collection.CollectionId == collectionId);

        return collection is null ? Errors.Collection.NotFound : collection;
    }

    public async Task<List<Collection>> GetCollections()
    {
        return await database.Collections.AsNoTracking().ToListAsync();
    }

    public async Task<ErrorOr<PagedList<Article>>> GetPublishedArticlesFromColelction(string collectionId, int page, int size, CancellationToken cancellationToken)
    {
        var foundCollection = await FindCollectionById(collectionId);

        if (foundCollection.IsError)
            return foundCollection.Errors;

        return await database.Collections
            .AsNoTracking()
            .Include(collection => collection.Articles)
            .Where(collection => collection.CollectionId == collectionId)
            .SelectMany(collection => collection.Articles)
            .Where(article => article.IsPublished)
            .OrderByDescending(article => article.IsPinned)
            .ThenByDescending(article => article.PublishDate)
            .ToPagedList(page, size, cancellationToken);
    }

    public async Task<bool> IsCollectionExists(string collectionId)
    {
        return await database.Collections.AnyAsync(collection => collection.CollectionId == collectionId);
    }

    public async Task<ErrorOr<Deleted>> DeleteCollection(string collectionId)
    {
        var collection = await FindCollectionById(collectionId);

        if (collection.IsError)
            return collection.Errors;

        database.Remove(collection);

        return Result.Deleted;
    }

    public async Task SaveChanges()
    {
        await database.SaveChangesAsync();
    }
}
