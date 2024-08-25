namespace Database.Repositories;

public class CollectionRepository(DatabaseContext database) : ICollectionRepository
{
    public async Task AddCollection(Collection newCollection)
    {
        await database.AddAsync(newCollection);
    }

    public async Task<Collection?> FindCollectionById(string collectionId)
    {
        return await database.Collections
            .Include(collection => collection.Articles)
            .SingleOrDefaultAsync(collection => collection.CollectionId == collectionId);
    }

    public async Task<List<Collection>> GetCollections()
    {
        return await database.Collections.AsNoTracking().ToListAsync();
    }

    public async Task<bool> IsCollectionExists(string collectionId)
    {
        return await database.Collections.AnyAsync(collection => collection.CollectionId == collectionId);
    }

    public async Task<bool> DeleteCollection(string collectionId)
    {
        var collection = await FindCollectionById(collectionId);

        if (collection is null)
            return false;

        database.Remove(collection);

        return true;
    }

    public async Task SaveChanges()
    {
        await database.SaveChangesAsync();
    }
}
