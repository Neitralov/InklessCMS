namespace WebAPI.IntegrationTests.CollectionsControllerEndpoints;

[Collection("Tests")]
public sealed class DeleteArticleFromCollectionTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
    private readonly CustomWebApplicationFactory _factory = factory;

    [Fact]
    public async Task ArticleCanBeDeletedFromCollection()
    {
        // Arrange
        var customClient = _factory.AuthorizeAs(UserTypes.Admin).CreateClient();

        const string collectionId = "collection-id";
        await customClient.PostAsJsonAsync(
            requestUri: "/api/collections",
            value: DataGenerator.Collection.GetCreateCollectionRequest() with { CollectionId = collectionId });

        const string articleId = "article-id";
        await customClient.PostAsJsonAsync(
            requestUri: "/api/articles",
            value: DataGenerator.Article.GetCreateRequest() with { ArticleId = articleId });
        await customClient.PostAsJsonAsync(
            requestUri: $"/api/collections/{collectionId}",
            value: DataGenerator.Collection.GetAddArticleToCollectionRequest() with { ArticleId = articleId });

        // Act
        var getCollectionBeforeArticleDeletionResponse =
            await customClient.GetAsync($"/api/collections/{collectionId}");

        var deleteArticleFromCollectionResponse =
            await customClient.DeleteAsync($"/api/collections/{collectionId}/articles/{articleId}");

        var getCollectionAfterArticleDeletionResponse =
            await customClient.GetAsync($"/api/collections/{collectionId}");

        // Assert
        (await getCollectionBeforeArticleDeletionResponse.Content.ReadFromJsonAsync<CollectionResponse>())?
            .Articles.Should().NotBeEmpty();
        deleteArticleFromCollectionResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        (await getCollectionAfterArticleDeletionResponse.Content.ReadFromJsonAsync<CollectionResponse>())?
            .Articles.Should().BeEmpty();
    }

    [Fact]
    public async Task ArticleCannotBeDeletedFromCollectionIfArticleDoesNotExist()
    {
        // Arrange
        var customClient = _factory.AuthorizeAs(UserTypes.Admin).CreateClient();
        const string articleId = "article-id";

        const string collectionId = "collection-id";
        await customClient.PostAsJsonAsync(
            requestUri: "/api/collections",
            value: DataGenerator.Collection.GetCreateCollectionRequest() with { CollectionId = collectionId });

        // Act
        var response = await customClient.DeleteAsync($"/api/collections/{collectionId}/articles/{articleId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ArticleCannotBeDeletedFromCollectionIfCollectionDoesNotExist()
    {
        // Arrange
        var customClient = _factory.AuthorizeAs(UserTypes.Admin).CreateClient();
        const string collectionId = "collection-id";

        const string articleId = "article-id";
        await customClient.PostAsJsonAsync(
            requestUri: "/api/articles",
            value: DataGenerator.Article.GetCreateRequest() with { ArticleId = articleId });

        // Act
        var response = await customClient.DeleteAsync($"/api/collections/{collectionId}/articles/{articleId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task OnlyAuthorizedUserCanDeleteArticleFromCollection()
    {
        // Arrange
        var client = _factory.CreateClient();
        const string collectionId = "collection-id";
        const string articleId = "article-id";

        // Act
        var response = await client.DeleteAsync($"/api/collections/{collectionId}/articles/{articleId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task OnlyUserWithCanManageArticlesClaimCanDeleteArticleFromCollection()
    {
        // Arrange
        var customClient = _factory.AuthorizeAs(UserTypes.User).CreateClient();
        const string collectionId = "collection-id";
        const string articleId = "article-id";

        // Act
        var response = await customClient.DeleteAsync($"/api/collections/{collectionId}/articles/{articleId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
