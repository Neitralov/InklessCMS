namespace WebAPI.IntegrationTests.CollectionsControllerEndpoints;

[Collection("Tests")]
public sealed class DeleteCollectionTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
    private readonly CustomWebApplicationFactory _factory = factory;

    [Fact]
    public async Task CollectionCanBeDeleted()
    {
        // Arrange
        var gqlClient = _factory.AuthorizeAs(UserTypes.Admin).CreateClient().ToGqlClient();

        const string collectionId = "collection-id";
        await gqlClient.CreateCollection(Inputs.Collection.CollectionInput with { CollectionId = collectionId });

        // Act
        await gqlClient.DeleteCollection(collectionId);
        var exception = await Should.ThrowAsync<GraphQLException>(async () =>
        {
            await gqlClient.GetCollection(collectionId);
        });
        
        // Assert
        exception.Message!.ShouldContain(Collection.Errors.NotFound.Code);
    }

    [Fact]
    public async Task CollectionCannotBeDeletedIfItDoesNotExist()
    {
        // Arrange
        var gqlClient = _factory.AuthorizeAs(UserTypes.Admin).CreateClient().ToGqlClient();
        const string collectionId = "collection-id";

        // Act
        var exception = await Should.ThrowAsync<GraphQLException>(async () =>
        {
            await gqlClient.GetCollection(collectionId);
        });
        
        // Assert
        exception.Message!.ShouldContain(Collection.Errors.NotFound.Code);
    }

    [Fact]
    public async Task CascadingDeletionOfArticlesWontOccurIfDeleteCollectionWithArticles()
    {
        // Arrange
        var gqlClient = _factory.AuthorizeAs(UserTypes.Admin).CreateClient().ToGqlClient();

        const string collectionId = "collection-id";
        await gqlClient.CreateCollection(Inputs.Collection.CollectionInput with { CollectionId = collectionId });

        const string articleId = "article-id";
        await gqlClient.CreateArticle(Inputs.Article.ArticleInput with { ArticleId = articleId });
        await gqlClient.AddArticleToCollection(collectionId, articleId);

        // Act
        await gqlClient.DeleteCollection(collectionId);
        var exception = await Should.ThrowAsync<GraphQLException>(async () =>
        {
            await gqlClient.GetCollection(collectionId);
        });
        var gqlResponse = await gqlClient.GetArticle(articleId);

        // Assert
        exception.Message!.ShouldContain(Collection.Errors.NotFound.Code);
        gqlResponse.ArticleId.ShouldBe(articleId);
    }

    [Fact]
    public async Task OnlyAuthorizedUserCanDeleteCollection()
    {
        // Arrange
        var gqlClient = _factory.CreateClient().ToGqlClient();
        const string collectionId = "collection-id";

        // Act
        var exception = await Should.ThrowAsync<GraphQLException>(async () =>
        {
            await gqlClient.DeleteCollection(collectionId);
        });

        // Assert
        exception.Message!.ShouldContain("The current user is not authorized to access this resource.");
    }

    [Fact]
    public async Task OnlyUserWithCanManageArticlesClaimCanDeleteCollection()
    {
        // Arrange
        var gqlClient = _factory.AuthorizeAs(UserTypes.User).CreateClient().ToGqlClient();
        const string collectionId = "collection-id";

        // Act
        var exception = await Should.ThrowAsync<GraphQLException>(async () =>
        {
            await gqlClient.DeleteCollection(collectionId);
        });

        // Assert
        exception.Message!.ShouldContain("The current user is not authorized to access this resource.");
    }
}
