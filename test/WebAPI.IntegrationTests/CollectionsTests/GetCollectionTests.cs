namespace WebAPI.IntegrationTests.CollectionsTests;

[Collection("Tests")]
public sealed class GetCollectionTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
    private readonly CustomWebApplicationFactory _factory = factory;

    [Fact]
    public async Task EmptyListWillBeReturnedIfCollectionIsEmpty()
    {
        // Arrange
        var gqlClient = _factory.AuthorizeAs(UserTypes.Admin).CreateClient().ToGqlClient();
        const string collectionId = "collection-id";

        await gqlClient.CreateCollection(Inputs.Collection.CollectionInput with { CollectionId = collectionId });

        // Act
        var gqlResponse = await gqlClient.GetCollection(collectionId);

        // Assert
        gqlResponse.Articles.ShouldBeEmpty();
    }

    [Fact]
    public async Task ArticlesWillBeReturnedIfCollectionContainsArticles()
    {
        // Arrange
        var gqlClient = _factory.AuthorizeAs(UserTypes.Admin).CreateClient().ToGqlClient();
        const string collectionId = "collection-id";
        const int numberOfArticles = 2;

        await gqlClient.CreateCollection(Inputs.Collection.CollectionInput with { CollectionId = collectionId });

        const string firstArticleId = "article-1";
        await gqlClient.CreateArticle(Inputs.Article.ArticleInput with
        {
            ArticleId = firstArticleId,
            IsPublished = true
        });
        await gqlClient.AddArticleToCollection(collectionId, firstArticleId);

        const string secondArticleId = "article-2";
        await gqlClient.CreateArticle(Inputs.Article.ArticleInput with
        {
            ArticleId = secondArticleId,
            IsPublished = false
        });
        await gqlClient.AddArticleToCollection(collectionId, secondArticleId);

        // Act
        var gqlResponse = await gqlClient.GetCollection(collectionId);

        // Assert
        gqlResponse?.Articles?.Count().ShouldBe(numberOfArticles);
    }

    [Fact]
    public async Task ArticlesWontBeReturnedIfCollectionDoesNotExist()
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
    public async Task OnlyAuthorizedUserCanGetCollection()
    {
        // Arrange
        var gqlClient = _factory.CreateClient().ToGqlClient();
        const string collectionId = "collection-id";

        // Act
        var exception = await Should.ThrowAsync<GraphQLException>(async () =>
        {
            await gqlClient.GetCollection(collectionId);
        });

        // Assert
        exception.Message!.ShouldContain("The current user is not authorized to access this resource.");
    }

    [Fact]
    public async Task OnlyUserWithCanManageArticlesClaimCanGetCollection()
    {
        // Arrange
        var gqlClient = _factory.AuthorizeAs(UserTypes.User).CreateClient().ToGqlClient();
        const string collectionId = "collection-id";

        // Act
        var exception = await Should.ThrowAsync<GraphQLException>(async () =>
        {
            await gqlClient.GetCollection(collectionId);
        });

        // Assert
        exception.Message!.ShouldContain("The current user is not authorized to access this resource.");
    }
}
