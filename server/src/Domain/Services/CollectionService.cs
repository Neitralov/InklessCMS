namespace Domain.Services;

public class CollectionService(ICollectionRepository collectionRepository, IArticleRepository articleRepository)
{
    public async Task<ErrorOr<Created>> StoreCollection(Collection newCollection)
    {
        if (await collectionRepository.IsCollectionExists(newCollection.CollectionId))
            return Errors.Collection.NonUniqueId;

        await collectionRepository.AddCollection(newCollection);
        await collectionRepository.SaveChanges();

        return Result.Created;
    }

    public async Task<ErrorOr<Success>> StoreArticleToCollection(string collectionId, string articleId)
    {
        var collection = await collectionRepository.FindCollectionById(collectionId);

        if (collection.IsError)
            return collection.Errors;

        var article = await articleRepository.FindArticleById(articleId);

        if (article.IsError)
            return article.Errors;

        if (collection.Value.Articles.Contains(article.Value))
            return Errors.Collection.ArticleAlreadyAdded;

        var result = collection.Value.AddArticle(article.Value);

        if (result == Result.Success)
            await collectionRepository.SaveChanges();

        return result;
    }

    public async Task<List<Collection>> GetCollections()
    {
        var collections = await collectionRepository.GetCollections();

        return collections;
    }

    public async Task<ErrorOr<Collection>> GetCollection(string collectionId)
    {
        var collection = await collectionRepository.FindCollectionById(collectionId);

        if (collection.IsError)
            return collection.Errors;

        return collection;
    }

    public async Task<ErrorOr<PagedList<Article>>> GetPublishedArticlesFromCollection(string collectionId, int page, int size, CancellationToken cancellationToken)
    {
        var publishedArticlesFromCollection = await collectionRepository.GetPublishedArticlesFromColelction(collectionId, page, size, cancellationToken);

        return publishedArticlesFromCollection;
    }

    public async Task<ErrorOr<Updated>> UpdateCollection(Collection updatedCollection)
    {
        var currentCollection = await collectionRepository.FindCollectionById(updatedCollection.CollectionId);

        if (currentCollection.IsError)
            return currentCollection.Errors;

        var result = currentCollection.Value.Update(updatedCollection);

        if (result == Result.Updated)
            await collectionRepository.SaveChanges();

        return result;
    }

    public async Task<ErrorOr<Deleted>> DeleteCollection(string collectionId)
    {
        var result = await collectionRepository.DeleteCollection(collectionId);

        if (result == Result.Deleted)
            await collectionRepository.SaveChanges();

        return result;
    }

    public async Task<ErrorOr<Deleted>> DeleteArticleFromCollection(string collectionId, string articleId)
    {
        var collection = await collectionRepository.FindCollectionById(collectionId);

        if (collection.IsError)
            return collection.Errors;

        var result = collection.Value.DeleteArticle(articleId);

        if (result == Result.Deleted)
            await collectionRepository.SaveChanges();

        return result;
    }
}
