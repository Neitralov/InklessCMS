namespace Domain.Interfaces;

public interface ICollectionRepository
{
    Task AddCollection(Collection newCollection);
    Task<ErrorOr<Collection>> FindCollectionById(string collectionId);
    Task<List<Collection>> GetCollections();
    Task<ErrorOr<PagedList<Article>>> GetPublishedArticlesFromColelction(string collectionId, int page, int size);
    Task<bool> IsCollectionExists(string collectionId);
    Task<ErrorOr<Deleted>> DeleteCollection(string collectionId);
    Task SaveChanges();
}
