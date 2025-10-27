namespace WebAPI.GraphQL.Mutations;

[GraphQLName("CollectionMutations")]
public sealed class GqlCollectionMutations
{
    [GraphQLName("createCollection")]
    [GraphQLDescription("Создать коллекцию")]
    [GqlAuthorize(Policy = "CanManageArticles")]
    public async Task<GqlCollection> CreateCollectionAsync(
        ICollectionRepository collectionRepository,
        GqlCollectionInput input)
    {
        var requestToCollectionResult = CreateCollectionFrom(input);

        if (requestToCollectionResult.IsError)
            throw new Exception(requestToCollectionResult.FirstError.Code);

        var collection = requestToCollectionResult.Value;

        if (await collectionRepository.IsCollectionExistsAsync(collection.CollectionId))
            throw new Exception(Collection.Errors.NonUniqueId.Code);

        await collectionRepository.AddCollectionAsync(collection);
        await collectionRepository.SaveChangesAsync();

        return collection.ToGqlCollection();
    }

    [GraphQLName("addArticleToCollection")]
    [GraphQLDescription("Добавить статью в коллекцию")]
    [GqlAuthorize(Policy = "CanManageArticles")]
    public async Task<GqlCollection> AddArticleToCollectionAsync(
        ICollectionRepository collectionRepository,
        IArticleRepository articleRepository,
        string collectionId,
        string articleId)
    {
        var collection = await collectionRepository.FindCollectionByIdAsync(collectionId);

        if (collection.IsError)
            throw new Exception(collection.FirstError.Code);

        var collectionArticle = collection.Value.FindArticleById(articleId);

        if (!collectionArticle.IsError)
            throw new Exception(Collection.Errors.ArticleAlreadyAdded.Code);

        if (collectionArticle.IsError && collectionArticle.FirstError != Collection.Errors.ArticleNotFound)
            throw new Exception(collectionArticle.FirstError.Code);

        var article = await articleRepository.FindArticleByIdAsync(articleId);

        if (article.IsError)
            throw new Exception(article.FirstError.Code);

        collection.Value.AddArticle(article.Value);
        await collectionRepository.SaveChangesAsync();

        return collection.Value.ToGqlCollection();
    }

    [GraphQLName("updateCollection")]
    [GraphQLDescription("Обновить коллекцию")]
    [GqlAuthorize(Policy = "CanManageArticles")]
    public async Task<GqlCollection> UpdateCollectionAsync(
        ICollectionRepository collectionRepository,
        GqlCollectionInput input)
    {
        var requestToCollectionResult = CreateCollectionFrom(input);

        if (requestToCollectionResult.IsError)
            throw new Exception(requestToCollectionResult.FirstError.Code);

        var updatedCollection = requestToCollectionResult.Value;

        var collection = await collectionRepository.FindCollectionByIdAsync(updatedCollection.CollectionId);

        if (collection.IsError)
            throw new Exception(collection.FirstError.Code);

        collection.Value.Update(updatedCollection);
        await collectionRepository.SaveChangesAsync();

        return collection.Value.ToGqlCollection();
    }

    [GraphQLName("deleteCollection")]
    [GraphQLDescription("Удалить коллекцию")]
    [GqlAuthorize(Policy = "CanManageArticles")]
    public async Task<string> DeleteCollectionAsync(ICollectionRepository collectionRepository, string collectionId)
    {
        var deleteCollectionResult = await collectionRepository.DeleteCollectionAsync(collectionId);

        if (deleteCollectionResult.IsError)
            throw new Exception(deleteCollectionResult.FirstError.Code);

        await collectionRepository.SaveChangesAsync();

        return collectionId;
    }

    [GraphQLName("deleteArticleFromCollection")]
    [GraphQLDescription("Удалить статью из коллекции")]
    [GqlAuthorize(Policy = "CanManageArticles")]
    public async Task<string> DeleteArticleFromCollectionAsync(
        ICollectionRepository collectionRepository,
        string collectionId,
        string articleId)
    {
        var collection = await collectionRepository.FindCollectionByIdAsync(collectionId);

        if (collection.IsError)
            throw new Exception(collection.FirstError.Code);

        var result = collection.Value.DeleteArticle(articleId);

        if (result.IsError)
            throw new Exception(result.FirstError.Code);

        await collectionRepository.SaveChangesAsync();

        return articleId;
    }


    private static ErrorOr<Collection> CreateCollectionFrom(GqlCollectionInput input)
    {
        return Collection.Create(
            collectionId: input.CollectionId,
            title: input.Title);
    }
}
