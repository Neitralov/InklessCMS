namespace WebAPI.IntegrationTests.ArticlesControllerEndpoints;

[Collection("Tests")]
public sealed class CreateArticleTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
    private readonly CustomWebApplicationFactory _factory = factory;

    [Fact]
    public async Task ArticleCanBeCreated()
    {
        // Arrange
        var customClient = _factory.AuthorizeAs(UserTypes.Admin).CreateClient();
        const string articleId = "article-id";

        // Act
        var response = await customClient.PostAsJsonAsync(
            requestUri: "/api/articles",
            value: DataGenerator.Article.GetCreateRequest() with { ArticleId = articleId });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        (await response.Content.ReadFromJsonAsync<ArticleResponse>())?.ArticleId.Should().Be(articleId);
    }

    [Fact]
    public async Task InvalidArticleCannotBeCreated()
    {
        // Arrange
        var customClient = _factory.AuthorizeAs(UserTypes.Admin).CreateClient();
        const string invalidArticleId = "Inv@lid-Id";

        // Act
        var response = await customClient.PostAsJsonAsync(
            requestUri: "/api/articles",
            value: DataGenerator.Article.GetCreateRequest() with { ArticleId = invalidArticleId });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task OnlyAuthorizedUserCanCreateArticle()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync(
            requestUri: "/api/articles",
            value: DataGenerator.Article.GetCreateRequest());

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task OnlyUserWithCanManageArticlesClaimCanCreateArticle()
    {
        // Arrange
        var customClient = _factory.AuthorizeAs(UserTypes.User).CreateClient();

        // Act
        var response = await customClient.PostAsJsonAsync(
            requestUri: "/api/articles",
            value: DataGenerator.Article.GetCreateRequest());

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
