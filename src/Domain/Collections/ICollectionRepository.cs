namespace Domain.Collections;

public interface ICollectionRepository
{
    Task AddCollectionAsync(Collection newCollection);
    Task<ErrorOr<Collection>> FindCollectionByIdAsync(string collectionId);
    Task<List<Collection>> GetCollectionsAsync();
    Task<ErrorOr<PagedList<Article>>> GetPublishedArticlesFromColelctionAsync(
        string collectionId,
        PageOptions pageOptions,
        CancellationToken cancellationToken);
    Task<bool> IsCollectionExistsAsync(string collectionId);
    Task<ErrorOr<Deleted>> DeleteCollectionAsync(string collectionId);
    Task SaveChangesAsync();
}
