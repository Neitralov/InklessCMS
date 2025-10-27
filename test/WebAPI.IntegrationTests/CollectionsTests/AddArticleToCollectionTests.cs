namespace WebAPI.IntegrationTests.CollectionsTests;

[Collection("Tests")]
public sealed class AddArticleToCollectionTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
    private readonly CustomWebApplicationFactory _factory = factory;

    [Fact]
    public async Task ArticleCanBeAddedToCollection()
    {
        // Arrange
        var gqlClient = _factory.AuthorizeAs(UserTypes.Admin).CreateClient().ToGqlClient();

        const string collectionId = "collection-id";
        await gqlClient.CreateCollection(Inputs.Collection.CollectionInput with { CollectionId = collectionId });

        const string articleId = "article-id";
        await gqlClient.CreateArticle(Inputs.Article.ArticleInput with { ArticleId = articleId });

        // Act
        var gqlResponse = await gqlClient.AddArticleToCollection(collectionId, articleId);

        // Assert
        gqlResponse.Articles?.Count().ShouldBe(1);
    }

    [Fact]
    public async Task SameArticleCannotBeAddedToCollection()
    {
        // Arrange
        var gqlClient = _factory.AuthorizeAs(UserTypes.Admin).CreateClient().ToGqlClient();

        const string collectionId = "collection-id";
        await gqlClient.CreateCollection(Inputs.Collection.CollectionInput with { CollectionId = collectionId });

        const string articleId = "article-id";
        await gqlClient.CreateArticle(Inputs.Article.ArticleInput with { ArticleId = articleId });
        await gqlClient.AddArticleToCollection(collectionId, articleId);

        // Act
        var exception = await Should.ThrowAsync<GraphQLException>(async () =>
        {
            await gqlClient.AddArticleToCollection(collectionId, articleId);
        });

        // Assert
        exception.Message!.ShouldContain(Collection.Errors.ArticleAlreadyAdded.Code);
    }

    [Fact]
    public async Task ArticleCanBeAddedInSeveralCollectionsAtOnce()
    {
        // Arrange
        var gqlClient = _factory.AuthorizeAs(UserTypes.Admin).CreateClient().ToGqlClient();

        const string firstCollectionId = "collection-1";
        await gqlClient.CreateCollection(Inputs.Collection.CollectionInput with { CollectionId = firstCollectionId });

        const string secondCollectionId = "collection-2";
        await gqlClient.CreateCollection(Inputs.Collection.CollectionInput with { CollectionId = secondCollectionId });

        const string articleId = "article-id";
        await gqlClient.CreateArticle(Inputs.Article.ArticleInput with { ArticleId = articleId });
        await gqlClient.AddArticleToCollection(firstCollectionId, articleId);

        // Act & Assert
        await Should.NotThrowAsync(async () =>
        {
            await gqlClient.AddArticleToCollection(secondCollectionId, articleId);
        });
    }

    [Fact]
    public async Task ArticleThatDoesNotExistCannotBeAddedToCollection()
    {
        // Arrange
        var gqlClient = _factory.AuthorizeAs(UserTypes.Admin).CreateClient().ToGqlClient();

        const string collectionId = "collection-id";
        await gqlClient.CreateCollection(Inputs.Collection.CollectionInput with { CollectionId = collectionId });

        const string articleId = "article-id";

        // Act
        var exception = await Should.ThrowAsync<GraphQLException>(async () =>
        {
            await gqlClient.AddArticleToCollection(collectionId, articleId);
        });

        // Assert
        exception.Message!.ShouldContain(Article.Errors.NotFound.Code);
    }

    [Fact]
    public async Task ArticleCannotBeAddedToCollectionThatDoesNotExist()
    {
        // Arrange
        var gqlClient = _factory.AuthorizeAs(UserTypes.Admin).CreateClient().ToGqlClient();
        const string collectionId = "collection-id";

        const string articleId = "article-id";
        await gqlClient.CreateArticle(Inputs.Article.ArticleInput with { ArticleId = articleId });

        // Act
        var exception = await Should.ThrowAsync<GraphQLException>(async () =>
        {
            await gqlClient.AddArticleToCollection(collectionId, articleId);
        });

        // Assert
        exception.Message!.ShouldContain(Collection.Errors.NotFound.Code);
    }

    [Fact]
    public async Task OnlyAuthorizedUserCanAddArticleToCollection()
    {
        // Arrange
        var gqlClient = _factory.CreateClient().ToGqlClient();
        const string collectionId = "collection-id";
        const string articleId = "article-id";

        // Act
        var exception = await Should.ThrowAsync<GraphQLException>(async () =>
        {
            await gqlClient.AddArticleToCollection(collectionId, articleId);
        });

        // Assert
        exception.Message!.ShouldContain("The current user is not authorized to access this resource.");
    }

    [Fact]
    public async Task OnlyUserWithCanManageArticlesClaimCanAddArticleToCollection()
    {
        // Arrange
        var gqlClient = _factory.AuthorizeAs(UserTypes.User).CreateClient().ToGqlClient();
        const string collectionId = "collection-id";
        const string articleId = "article-id";

        // Act
        var exception = await Should.ThrowAsync<GraphQLException>(async () =>
        {
            await gqlClient.AddArticleToCollection(collectionId, articleId);
        });

        // Assert
        exception.Message!.ShouldContain("The current user is not authorized to access this resource.");
    }
}
