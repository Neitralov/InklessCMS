namespace WebAPI.GraphQL.Mutations;

[GraphQLName("CollectionMutations")]
public sealed class GqlCollectionMutations
{
    [GraphQLName("createCollection")]
    [GraphQLDescription("Создать коллекцию")]
    [GqlAuthorize(Policy = "CanManageArticles")]
    public async Task<GqlCollection> CreateCollectionAsync(
        [Service] ICollectionRepository collectionRepository,
        CreateCollectionRequest request)
    {
        var requestToCollectionResult = CreateCollectionFrom(request);

        if (requestToCollectionResult.IsError)
            throw new Exception(requestToCollectionResult.FirstError.Code);

        var collection = requestToCollectionResult.Value;

        if (await collectionRepository.IsCollectionExistsAsync(collection.CollectionId))
            throw new Exception(Collection.Errors.NonUniqueId.Code);

        await collectionRepository.AddCollectionAsync(collection);
        await collectionRepository.SaveChangesAsync();

        return new GqlCollection(collection);
    }

    [GraphQLName("addArticleToCollection")]
    [GraphQLDescription("Добавить статью в коллекцию")]
    [GqlAuthorize(Policy = "CanManageArticles")]
    public async Task<GqlCollection> AddArticleToCollectionAsync(
        [Service] ICollectionRepository collectionRepository,
        [Service] IArticleRepository articleRepository,
        string collectionId,
        AddArticleToCollectionRequest request
    )
    {
        var collection = await collectionRepository.FindCollectionByIdAsync(collectionId);

        if (collection.IsError)
            throw new Exception(collection.FirstError.Code);

        var article = await articleRepository.FindArticleByIdAsync(request.ArticleId);

        if (article.IsError)
            throw new Exception(article.FirstError.Code);

        if (collection.Value.Articles.Contains(article.Value))
            throw new Exception(Collection.Errors.ArticleAlreadyAdded.Code);

        collection.Value.AddArticle(article.Value);
        await collectionRepository.SaveChangesAsync();

        return new GqlCollection(collection.Value);
    }

    [GraphQLName("updateCollection")]
    [GraphQLDescription("Обновить коллекцию")]
    [GqlAuthorize(Policy = "CanManageArticles")]
    public async Task<GqlCollection> UpdateCollectionAsync(
        [Service] ICollectionRepository collectionRepository,
        UpdateCollectionRequest request)
    {
        var requestToCollectionResult = CreateCollectionFrom(request);

        if (requestToCollectionResult.IsError)
            throw new Exception(requestToCollectionResult.FirstError.Code);

        var updatedCollection = requestToCollectionResult.Value;

        var collection = await collectionRepository.FindCollectionByIdAsync(updatedCollection.CollectionId);

        if (collection.IsError)
            throw new Exception(collection.FirstError.Code);

        collection.Value.Update(updatedCollection);
        await collectionRepository.SaveChangesAsync();

        return new GqlCollection(collection.Value);
    }

    [GraphQLName("deleteCollection")]
    [GraphQLDescription("Удалить коллекцию")]
    [GqlAuthorize(Policy = "CanManageArticles")]
    public async Task<string> DeleteCollectionAsync(
        [Service] ICollectionRepository collectionRepository,
        string collectionId)
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
        [Service] ICollectionRepository collectionRepository,
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


    private static ErrorOr<Collection> CreateCollectionFrom(CreateCollectionRequest request)
    {
        return Collection.Create(
            collectionId: request.CollectionId,
            title: request.Title);
    }

    private static ErrorOr<Collection> CreateCollectionFrom(UpdateCollectionRequest request)
    {
        return Collection.Create(
            collectionId: request.CollectionId,
            title: request.Title);
    }
}
