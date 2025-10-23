namespace WebAPI.IntegrationTests.CollectionsControllerEndpoints;

[Collection("Tests")]
public sealed class DeleteArticleFromCollectionTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
    private readonly CustomWebApplicationFactory _factory = factory;

    [Fact]
    public async Task ArticleCanBeDeletedFromCollection()
    {
        // Arrange
        var gqlClient = _factory.AuthorizeAs(UserTypes.Admin).CreateClient().ToGqlClient();

        const string collectionId = "collection-id";
        await gqlClient.CreateCollection(Requests.Collection.CollectionInput with { CollectionId = collectionId });

        const string articleId = "article-id";
        await gqlClient.CreateArticle(Requests.Article.ArticleInput with { ArticleId = articleId });
        await gqlClient.AddArticleToCollection(collectionId, articleId);

        // Act
        var getCollectionBeforeArticleDeletionResponse = await gqlClient.GetCollection(collectionId);
        await gqlClient.DeleteArticleFromCollection(collectionId, articleId);
        var getCollectionAfterArticleDeletionResponse = await gqlClient.GetCollection(collectionId);

        // Assert
        getCollectionBeforeArticleDeletionResponse.Articles.ShouldNotBeEmpty();
        getCollectionAfterArticleDeletionResponse.Articles.ShouldBeEmpty();
    }

    [Fact]
    public async Task ArticleCannotBeDeletedFromCollectionIfArticleDoesNotExist()
    {
        // Arrange
        var gqlClient = _factory.AuthorizeAs(UserTypes.Admin).CreateClient().ToGqlClient();
        const string articleId = "article-id";

        const string collectionId = "collection-id";
        await gqlClient.CreateCollection(Requests.Collection.CollectionInput with { CollectionId = collectionId });

        // Act
        var exception = await Should.ThrowAsync<GraphQLHttpRequestException>(async () =>
        {
            await gqlClient.DeleteArticleFromCollection(collectionId, articleId);
        });

        // Assert
        exception.Content!.ShouldContain(Collection.Errors.ArticleNotFound.Code);
    }

    [Fact]
    public async Task ArticleCannotBeDeletedFromCollectionIfCollectionDoesNotExist()
    {
        // Arrange
        var gqlClient = _factory.AuthorizeAs(UserTypes.Admin).CreateClient().ToGqlClient();
        const string collectionId = "collection-id";

        const string articleId = "article-id";
        await gqlClient.CreateArticle(Requests.Article.ArticleInput with { ArticleId = articleId });

        // Act
        var exception = await Should.ThrowAsync<GraphQLHttpRequestException>(async () =>
        {
            await gqlClient.DeleteArticleFromCollection(collectionId, articleId);
        });

        // Assert
        exception.Content!.ShouldContain(Collection.Errors.NotFound.Code);
    }

    [Fact]
    public async Task OnlyAuthorizedUserCanDeleteArticleFromCollection()
    {
        // Arrange
        var gqlClient = _factory.CreateClient().ToGqlClient();
        const string collectionId = "collection-id";
        const string articleId = "article-id";

        // Act
        var exception = await Should.ThrowAsync<GraphQLHttpRequestException>(async () =>
        {
            await gqlClient.DeleteArticleFromCollection(collectionId, articleId);
        });

        // Assert
        exception.Content!.ShouldContain("AUTH_NOT_AUTHORIZED");
    }

    [Fact]
    public async Task OnlyUserWithCanManageArticlesClaimCanDeleteArticleFromCollection()
    {
        // Arrange
        var gqlClient = _factory.AuthorizeAs(UserTypes.User).CreateClient().ToGqlClient();
        const string collectionId = "collection-id";
        const string articleId = "article-id";

        // Act
        var exception = await Should.ThrowAsync<GraphQLHttpRequestException>(async () =>
        {
            await gqlClient.DeleteArticleFromCollection(collectionId, articleId);
        });

        // Assert
        exception.Content!.ShouldContain("AUTH_NOT_AUTHORIZED");
    }
}
