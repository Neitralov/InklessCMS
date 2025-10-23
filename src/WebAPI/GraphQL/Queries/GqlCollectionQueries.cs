namespace WebAPI.GraphQL.Queries;

[GraphQLName("CollectionQueries")]
public sealed class GqlCollectionQueries
{
    [GraphQLName("collections")]
    [GraphQLDescription("Список коллекций")]
    public async Task<GqlCollection[]> GetCollectionsAsync([Service] ICollectionRepository collectionRepository)
    {
        var collections = await collectionRepository.GetCollectionsAsync();

        return [.. collections.Select(collection => collection.ToGqlCollection())];
    }

    [GraphQLName("collection")]
    [GraphQLDescription("Коллекция")]
    [GqlAuthorize(Policy = "CanManageArticles")]
    public async Task<GqlCollection> GetCollectionAsync(
        [Service] ICollectionRepository collectionRepository,
        string collectionId)
    {
        var getCollectionResult = await collectionRepository.FindCollectionByIdAsync(collectionId);

        if (getCollectionResult.IsError)
            throw new Exception(getCollectionResult.FirstError.Code);

        var collection = getCollectionResult.Value;

        return collection.ToGqlCollection();
    }

    [GraphQLName("publishedArticlesFromCollection")]
    [GraphQLDescription("Опубликованные статьи из коллекции")]
    public async Task<GqlArticle[]> GetPublishedArticlesFromCollectionAsync(
        [Service] ICollectionRepository collectionRepository,
        [Service] IHttpContextAccessor httpContextAccessor,
        string collectionId,
        PageOptions pageOptions,
        CancellationToken cancellationToken)
    {
        var getPublishedArticlesFromCollectionResult =
            await collectionRepository.GetPublishedArticlesFromColelctionAsync(
                collectionId,
                pageOptions,
                cancellationToken);

        if (getPublishedArticlesFromCollectionResult.IsError)
            throw new Exception(getPublishedArticlesFromCollectionResult.FirstError.Code);

        var publishedArticlesFromCollection = getPublishedArticlesFromCollectionResult.Value;

        httpContextAccessor.HttpContext?.Response.Headers
            .Append("X-Total-Count", publishedArticlesFromCollection.TotalCount.ToString());

        return [.. publishedArticlesFromCollection.Select(article => article.ToGqlArticle())];
    }
}
