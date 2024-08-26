namespace Domain.Interfaces;

public interface ICollectionRepository
{
    Task AddCollection(Collection newCollection);
    Task<Collection?> FindCollectionById(string collectionId);
    Task<List<Collection>> GetCollections();
    Task<bool> IsCollectionExists(string collectionId);
    Task<bool> DeleteCollection(string collectionId);
    Task SaveChanges();
}
