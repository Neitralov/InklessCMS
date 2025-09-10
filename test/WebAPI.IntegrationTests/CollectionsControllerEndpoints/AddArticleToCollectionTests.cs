namespace WebAPI.IntegrationTests.CollectionsControllerEndpoints;

[Collection("Tests")]
public sealed class AddArticleToCollectionTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
    private readonly CustomWebApplicationFactory _factory = factory;

    [Fact]
    public async Task ArticleCanBeAddedToCollection()
    {
        // Arrange
        var customClient = _factory.AuthorizeAs(UserTypes.Admin).CreateClient();

        const string collectionId = "collection-id";
        await customClient.PostAsJsonAsync(
            requestUri: "/api/collections",
            value: Requests.Collection.GetCreateCollectionRequest() with { CollectionId = collectionId });

        const string articleId = "article-id";
        await customClient.PostAsJsonAsync(
            requestUri: "/api/articles",
            value: Requests.Article.ArticleInput with { ArticleId = articleId });

        // Act
        var addArticleToCollectionResponse = await customClient.PostAsJsonAsync(
            requestUri: $"/api/collections/{collectionId}",
            value: Requests.Collection.GetAddArticleToCollectionRequest() with { ArticleId = articleId });

        var getArticlesFromCollectionResponse = await customClient.GetAsync($"api/collections/{collectionId}");

        // Assert
        addArticleToCollectionResponse.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        (await getArticlesFromCollectionResponse.Content.ReadFromJsonAsync<CollectionResponse>())
            ?.Articles.Count().ShouldBe(1);
    }

    [Fact]
    public async Task SameArticleCannotBeAddedToCollection()
    {
        // Arrange
        var customClient = _factory.AuthorizeAs(UserTypes.Admin).CreateClient();

        const string collectionId = "collection-id";
        await customClient.PostAsJsonAsync(
            requestUri: "/api/collections",
            value: Requests.Collection.GetCreateCollectionRequest() with { CollectionId = collectionId });

        const string articleId = "article-id";
        await customClient.PostAsJsonAsync(
            requestUri: "/api/articles",
            value: Requests.Article.ArticleInput with { ArticleId = articleId });
        await customClient.PostAsJsonAsync(
            requestUri: $"/api/collections/{collectionId}",
            value: Requests.Collection.GetAddArticleToCollectionRequest() with { ArticleId = articleId });

        // Act
        var response = await customClient.PostAsJsonAsync(
            requestUri: $"/api/collections/{collectionId}",
            value: Requests.Collection.GetAddArticleToCollectionRequest() with { ArticleId = articleId });

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ArticleCanBeAddedInSeveralCollectionsAtOnce()
    {
        // Arrange
        var customClient = _factory.AuthorizeAs(UserTypes.Admin).CreateClient();

        const string firstCollectionId = "collection-1";
        await customClient.PostAsJsonAsync(
            requestUri: "/api/collections",
            value: Requests.Collection.GetCreateCollectionRequest() with { CollectionId = firstCollectionId });

        const string secondCollectionId = "collection-2";
        await customClient.PostAsJsonAsync(
            requestUri: "/api/collections",
            value: Requests.Collection.GetCreateCollectionRequest() with { CollectionId = secondCollectionId });

        const string articleId = "article-id";
        await customClient.PostAsJsonAsync(
            requestUri: "/api/articles",
            value: Requests.Article.ArticleInput with { ArticleId = articleId });
        await customClient.PostAsJsonAsync(
            requestUri: $"/api/collections/{firstCollectionId}",
            value: Requests.Collection.GetAddArticleToCollectionRequest() with { ArticleId = articleId });

        // Act
        var response = await customClient.PostAsJsonAsync(
            requestUri: $"/api/collections/{secondCollectionId}",
            value: Requests.Collection.GetAddArticleToCollectionRequest() with { ArticleId = articleId });

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task ArticleThatDoesNotExistCannotBeAddedToCollection()
    {
        // Arrange
        var customClient = _factory.AuthorizeAs(UserTypes.Admin).CreateClient();

        const string collectionId = "collection-id";
        await customClient.PostAsJsonAsync(
            requestUri: "/api/collections",
            value: Requests.Collection.GetCreateCollectionRequest() with { CollectionId = collectionId });

        // Act
        var response = await customClient.PostAsJsonAsync(
            requestUri: $"/api/collections/{collectionId}",
            value: Requests.Collection.GetAddArticleToCollectionRequest());

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ArticleCannotBeAddedToCollectionThatDoesNotExist()
    {
        // Arrange
        var customClient = _factory.AuthorizeAs(UserTypes.Admin).CreateClient();
        const string collectionId = "collection-id";

        const string articleId = "article-id";
        await customClient.PostAsJsonAsync(
            requestUri: "/api/articles",
            value: Requests.Article.ArticleInput with { ArticleId = articleId });

        // Act
        var response = await customClient.PostAsJsonAsync(
            requestUri: $"/api/collections/{collectionId}",
            value: Requests.Collection.GetAddArticleToCollectionRequest() with { ArticleId = articleId });

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task OnlyAuthorizedUserCanAddArticleToCollection()
    {
        // Arrange
        var client = _factory.CreateClient();
        const string collectionId = "collection-id";

        // Act
        var response = await client.PostAsJsonAsync(
            requestUri: $"/api/collections/{collectionId}",
            value: Requests.Collection.GetAddArticleToCollectionRequest());

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task OnlyUserWithCanManageArticlesClaimCanAddArticleToCollection()
    {
        // Arrange
        var customClient = _factory.AuthorizeAs(UserTypes.User).CreateClient();
        const string collectionId = "collection-id";

        // Act
        var response = await customClient.PostAsJsonAsync(
            requestUri: $"/api/collections/{collectionId}",
            value: Requests.Collection.GetAddArticleToCollectionRequest());

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
}
