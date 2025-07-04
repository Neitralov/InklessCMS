namespace WebAPI.IntegrationTests.ArticlesControllerEndpoints;

[Collection("Tests")]
public sealed class UpdateArticleTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
    private readonly CustomWebApplicationFactory _factory = factory;

    [Fact]
    public async Task ArticleCanBeUpdated()
    {
        // Arrange
        var customClient = _factory.AuthorizeAs(UserTypes.Admin).CreateClient();
        const string articleId = "article-id";
        const string defaultTitle = "Default title";
        const string updatedTitle = "Updated title";

        await customClient.PostAsJsonAsync(
            requestUri: "/api/articles",
            value: DataGenerator.Article.GetCreateRequest() with { ArticleId = articleId, Title = defaultTitle });

        // Act
        var getArticleResponse1 = await customClient.GetAsync($"/api/articles/{articleId}");

        var updateArticleResponse = await customClient.PutAsJsonAsync(
            requestUri: "api/articles",
            value: DataGenerator.Article.GetUpdateRequest() with { ArticleId = articleId, Title = updatedTitle });

        var getArticleResponse2 = await customClient.GetAsync($"/api/articles/{articleId}");

        // Assert
        updateArticleResponse.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        var firstContent = await getArticleResponse1.Content.ReadFromJsonAsync<ArticleResponse>();
        var secondContent = await getArticleResponse2.Content.ReadFromJsonAsync<ArticleResponse>();
        firstContent?.Title.ShouldNotBe(secondContent?.Title);
    }

    [Fact]
    public async Task ArticleCannotBeUpdatedIfItDoesNotExist()
    {
        // Arrange
        var customClient = _factory.AuthorizeAs(UserTypes.Admin).CreateClient();
        const string articleId = "article-id";

        // Act
        var updateArticleResponse = await customClient.PutAsJsonAsync(
            requestUri: "api/articles",
            value: DataGenerator.Article.GetUpdateRequest() with { ArticleId = articleId });

        // Assert
        updateArticleResponse.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ArticleCannotBeUpdatedWithInvalidData()
    {
        // Arrange
        var customClient = _factory.AuthorizeAs(UserTypes.Admin).CreateClient();
        const string articleId = "article-id";
        const string tooShortTitle = "Aa";

        await customClient.PostAsJsonAsync(
            requestUri: "/api/articles",
            value: DataGenerator.Article.GetCreateRequest() with { ArticleId = articleId });

        // Act
        var updateArticleResponse = await customClient.PutAsJsonAsync(
            requestUri: "api/articles",
            value: DataGenerator.Article.GetUpdateRequest() with { ArticleId = articleId, Title = tooShortTitle });

        // Assert
        updateArticleResponse.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task OnlyAuthorizedUserCanUpdateArticle()
    {
        // Arrange
        var client = _factory.CreateClient();
        const string articleId = "article-id";

        // Act
        var updateArticleResponse = await client.PutAsJsonAsync(
            requestUri: "api/articles",
            value: DataGenerator.Article.GetUpdateRequest() with { ArticleId = articleId });

        // Assert
        updateArticleResponse.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task OnlyUserWithCanManageArticlesClaimCanUpdateArticle()
    {
        // Arrange
        var customClient = _factory.AuthorizeAs(UserTypes.User).CreateClient();
        const string articleId = "article-id";

        // Act
        var updateArticleResponse = await customClient.PutAsJsonAsync(
            requestUri: "api/articles",
            value: DataGenerator.Article.GetUpdateRequest() with { ArticleId = articleId });

        // Assert
        updateArticleResponse.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
}
