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

        if (collection is null)
            return Errors.Collection.NotFound;

        var article = await articleRepository.FindArticleById(articleId);

        if (article is null)
            return Errors.Article.NotFound;

        if (collection.Articles.Contains(article))
            return Errors.Collection.ArticleAlreadyAdded;

        var result = collection.AddArticle(article);

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

        if (collection is null)
            return Errors.Collection.NotFound;

        return collection;
    }

    public async Task<ErrorOr<Updated>> UpdateCollection(Collection updatedCollection)
    {
        var currentCollection = await collectionRepository.FindCollectionById(updatedCollection.CollectionId);

        if (currentCollection is null)
            return Errors.Collection.NotFound;

        var result = currentCollection.Update(updatedCollection);

        if (result == Result.Updated)
            await collectionRepository.SaveChanges();

        return result;
    }

    public async Task<ErrorOr<Deleted>> DeleteCollection(string collectionId)
    {
        var result = await collectionRepository.DeleteCollection(collectionId);

        if (result)
            await collectionRepository.SaveChanges();

        return result ? Result.Deleted : Errors.Collection.NotFound;
    }

    public async Task<ErrorOr<Deleted>> DeleteArticleFromCollection(string collectionId, string articleId)
    {
        var collection = await collectionRepository.FindCollectionById(collectionId);

        if (collection is null)
            return Errors.Collection.NotFound;

        var result = collection.DeleteArticle(articleId);

        if (result == Result.Deleted)
            await collectionRepository.SaveChanges();

        return result;
    }
}
