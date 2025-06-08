namespace WebAPI.IntegrationTests.ArticlesControllerEndpoints;

[Collection("Tests")]
public sealed class IncreaseViewsCounterTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
    private readonly CustomWebApplicationFactory _factory = factory;

    [Fact]
    public async Task ViewsCounterCanBeIncreased()
    {
        // Arrange
        var customClient = _factory.AuthorizeAs(UserTypes.Admin).CreateClient();
        const string articleId = "article-id";
        const int totalViews = 2;

        await customClient.PostAsJsonAsync(
            requestUri: "/api/articles",
            value: DataGenerator.Article.GetCreateRequest() with { ArticleId = articleId });

        // Act
        var increaseViewsResponse1 = await customClient.PatchAsync($"/api/articles/{articleId}/increase-views", null);
        var increaseViewsResponse2 = await customClient.PatchAsync($"/api/articles/{articleId}/increase-views", null);
        var getArticleResponse = await customClient.GetAsync($"/api/articles/{articleId}");

        // Assert
        increaseViewsResponse1.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        increaseViewsResponse2.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        (await getArticleResponse.Content.ReadFromJsonAsync<ArticleResponse>())?.Views.ShouldBe(totalViews);
    }

    [Fact]
    public async Task ViewsCounterCannotBeIncreasedIfArticleDoesNotExist()
    {
        // Arrange
        var client = _factory.CreateClient();
        const string articleId = "article-id";

        // Act
        var response = await client.PatchAsync($"/api/articles/{articleId}/increase-views", null);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
