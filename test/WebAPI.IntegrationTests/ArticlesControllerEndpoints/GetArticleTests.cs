namespace WebAPI.IntegrationTests.ArticlesControllerEndpoints;

[Collection("Tests")]
public sealed class GetArticleTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
    private readonly CustomWebApplicationFactory _factory = factory;

    [Fact]
    public async Task ArticleWillBeReturnedIfItExists()
    {
        // Arrange
        var client = _factory.CreateClient();
        var customClient = _factory.AuthorizeAs(UserTypes.Admin).CreateClient();

        const string firstArticleId = "article-1";
        await customClient.PostAsJsonAsync(
            requestUri: "/api/articles",
            value: DataGenerator.Article.GetCreateRequest() with { ArticleId = firstArticleId, IsPublished = false });

        const string secondArticleId = "article-2";
        await customClient.PostAsJsonAsync(
            requestUri: "/api/articles",
            value: DataGenerator.Article.GetCreateRequest() with { ArticleId = secondArticleId, IsPublished = true });

        // Act
        var response1 = await customClient.GetAsync($"/api/articles/{firstArticleId}");
        var response2 = await client.GetAsync($"/api/articles/{secondArticleId}");

        // Assert
        response1.StatusCode.ShouldBe(HttpStatusCode.OK);
        (await response1.Content.ReadFromJsonAsync<ArticleResponse>())?.ArticleId.ShouldBe(firstArticleId);

        response2.StatusCode.ShouldBe(HttpStatusCode.OK);
        (await response2.Content.ReadFromJsonAsync<ArticleResponse>())?.ArticleId.ShouldBe(secondArticleId);
    }

    [Fact]
    public async Task ArticleWontBeReturnedIfItDoesNotExist()
    {
        // Arrange
        var client = _factory.CreateClient();
        const string articleId = "article-id";

        // Act
        var response = await client.GetAsync($"/api/articles/{articleId}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DraftWontBeReturnedIfYouHaveNoCanManageArticlesClaim()
    {
        // Arrange
        var client = _factory.CreateClient();
        var customClient = _factory.AuthorizeAs(UserTypes.Admin).CreateClient();
        const string articleId = "article-id";

        await customClient.PostAsJsonAsync(
            requestUri: "/api/articles",
            value: DataGenerator.Article.GetCreateRequest() with { ArticleId = articleId, IsPublished = false });

        // Act
        var response = await client.GetAsync($"/api/articles/{articleId}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
