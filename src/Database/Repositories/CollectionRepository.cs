namespace Database.Repositories;

public sealed class CollectionRepository(DatabaseContext database) : BaseRepository(database), ICollectionRepository
{
    private readonly DbSet<Collection> _collections = database.Set<Collection>();

    public async Task AddCollectionAsync(Collection newCollection) => await _collections.AddAsync(newCollection);

    public async Task<ErrorOr<Collection>> FindCollectionByIdAsync(string collectionId)
    {
        return await _collections
            .Include(collection => collection.Articles)
            .SingleOrDefaultAsync(collection => collection.CollectionId == collectionId) ??
            Collection.Errors.NotFound.ToErrorOr<Collection>();
    }

    public async Task<List<Collection>> GetCollectionsAsync() => await _collections.AsNoTracking().ToListAsync();

    public async Task<ErrorOr<PagedList<Article>>> GetPublishedArticlesFromColelctionAsync(
        string collectionId,
        PageOptions pageOptions,
        CancellationToken cancellationToken)
    {
        var foundCollection = await FindCollectionByIdAsync(collectionId);

        if (foundCollection.IsError)
            return foundCollection.Errors;

        return await _collections
            .AsNoTracking()
            .Include(collection => collection.Articles)
            .Where(collection => collection.CollectionId == collectionId)
            .SelectMany(collection => collection.Articles)
            .Where(article => article.IsPublished)
            .OrderByDescending(article => article.IsPinned)
            .ThenByDescending(article => article.PublishDate!.Value)
            .ToPagedList(pageOptions, cancellationToken);
    }

    public async Task<bool> IsCollectionExistsAsync(string collectionId)
    {
        return await _collections.AnyAsync(collection => collection.CollectionId == collectionId);
    }

    public async Task<ErrorOr<Deleted>> DeleteCollectionAsync(string collectionId)
    {
        var collection = await FindCollectionByIdAsync(collectionId);

        if (collection.IsError)
            return collection.Errors;

        _collections.Remove(collection.Value);

        return Result.Deleted;
    }
}
