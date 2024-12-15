namespace Domain.Collections;

public sealed class CollectionService(ICollectionRepository collectionRepository, IArticleRepository articleRepository)
{
    public async Task<ErrorOr<Created>> AddCollection(Collection newCollection)
    {
        if (await collectionRepository.IsCollectionExists(newCollection.CollectionId))
            return Collection.Errors.NonUniqueId;

        await collectionRepository.AddCollection(newCollection);
        await collectionRepository.SaveChanges();

        return Result.Created;
    }

    public async Task<ErrorOr<Success>> AddArticleToCollection(string collectionId, string articleId)
    {
        var collection = await collectionRepository.FindCollectionById(collectionId);

        if (collection.IsError)
            return collection.Errors;

        var article = await articleRepository.FindArticleById(articleId);

        if (article.IsError)
            return article.Errors;

        if (collection.Value.Articles.Contains(article.Value))
            return Collection.Errors.ArticleAlreadyAdded;

        var result = collection.Value.AddArticle(article.Value);

        if (result == Result.Success)
            await collectionRepository.SaveChanges();

        return result;
    }

    public async Task<List<Collection>> GetCollections() => await collectionRepository.GetCollections();

    public async Task<ErrorOr<Collection>> GetCollection(string collectionId)
    {
        var collection = await collectionRepository.FindCollectionById(collectionId);

        return collection.IsError ? collection.Errors : collection;
    }

    public async Task<ErrorOr<PagedList<Article>>> GetPublishedArticlesFromCollection(
        string collectionId,
        PageOptions pageOptions,
        CancellationToken cancellationToken) =>
        await collectionRepository.GetPublishedArticlesFromColelction(collectionId, pageOptions, cancellationToken);

    public async Task<ErrorOr<Updated>> UpdateCollection(Collection updatedCollection)
    {
        var collection = await collectionRepository.FindCollectionById(updatedCollection.CollectionId);

        if (collection.IsError)
            return collection.Errors;

        var result = collection.Value.Update(updatedCollection);

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