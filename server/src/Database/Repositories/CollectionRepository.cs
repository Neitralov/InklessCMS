namespace Database.Repositories;

public sealed class CollectionRepository(DatabaseContext database) : BaseRepository(database), ICollectionRepository
{
    private readonly DatabaseContext _database = database;

    public async Task AddCollection(Collection newCollection) => await _database.Collections.AddAsync(newCollection);

    public async Task<ErrorOr<Collection>> FindCollectionById(string collectionId) =>
        await _database.Collections
            .Include(collection => collection.Articles)
            .SingleOrDefaultAsync(collection => collection.CollectionId == collectionId) ??
        Domain.Collections.Errors.Collection.NotFound.ToErrorOr<Collection>();

    public async Task<List<Collection>> GetCollections() => await _database.Collections.AsNoTracking().ToListAsync();

    public async Task<ErrorOr<PagedList<Article>>> GetPublishedArticlesFromColelction(
        string collectionId,
        PageOptions pageOptions,
        CancellationToken cancellationToken)
    {
        var foundCollection = await FindCollectionById(collectionId);

        if (foundCollection.IsError)
            return foundCollection.Errors;

        return await _database.Collections
            .AsNoTracking()
            .Include(collection => collection.Articles)
            .Where(collection => collection.CollectionId == collectionId)
            .SelectMany(collection => collection.Articles)
            .Where(article => article.IsPublished)
            .OrderByDescending(article => article.IsPinned)
            .ThenByDescending(article => article.PublishDate)
            .ToPagedList(pageOptions, cancellationToken);
    }

    public async Task<bool> IsCollectionExists(string collectionId) =>
        await _database.Collections.AnyAsync(collection => collection.CollectionId == collectionId);

    public async Task<ErrorOr<Deleted>> DeleteCollection(string collectionId)
    {
        var collection = await FindCollectionById(collectionId);

        if (collection.IsError)
            return collection.Errors;

        _database.Collections.Remove(collection.Value);

        return Result.Deleted;
    }
}