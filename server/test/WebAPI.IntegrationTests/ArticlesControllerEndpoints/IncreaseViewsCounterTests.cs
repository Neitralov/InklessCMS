namespace WebAPI.IntegrationTests.ArticlesControllerEndpoints;

[Collection("Tests")]
public sealed class IncreaseViewsCounterTests(CustomWebApplicationFactory factory) : IAsyncLifetime
{
    public Task InitializeAsync() => Task.CompletedTask;
    
    public async Task DisposeAsync() => await factory.ResetDatabaseAsync();

    [Fact]
    public async Task ViewsCounterCanBeIncreased()
    {
        // Arrange
        var customClient = factory.AuthorizeAs(UserTypes.Admin).CreateClient();
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
        increaseViewsResponse1.StatusCode.Should().Be(HttpStatusCode.NoContent);
        increaseViewsResponse2.StatusCode.Should().Be(HttpStatusCode.NoContent);
        (await getArticleResponse.Content.ReadFromJsonAsync<ArticleResponse>())?.Views.Should().Be(totalViews);
    }
    
    [Fact]
    public async Task ViewsCounterCannotBeIncreasedIfArticleDoesNotExist()
    {
        // Arrange
        var client = factory.CreateClient();
        const string articleId = "article-id";
       
        // Act
        var response = await client.PatchAsync($"/api/articles/{articleId}/increase-views", null);
       
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}