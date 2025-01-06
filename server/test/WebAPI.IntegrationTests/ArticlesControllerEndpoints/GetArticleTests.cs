namespace WebAPI.IntegrationTests.ArticlesControllerEndpoints;

[Collection("Tests")]
public sealed class GetArticleTests(CustomWebApplicationFactory factory) : IAsyncLifetime
{
    public Task InitializeAsync() => Task.CompletedTask;
    
    public async Task DisposeAsync() => await factory.ResetDatabaseAsync();

    [Fact]
    public async Task ArticleWillBeReturnedIfItExists()
    {
       // Arrange
       var client = factory.CreateClient();
       var customClient = factory.AuthorizeAs(UserTypes.Admin).CreateClient();

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
       response1.StatusCode.Should().Be(HttpStatusCode.OK);
       (await response1.Content.ReadFromJsonAsync<ArticleResponse>())?.ArticleId.Should().Be(firstArticleId);
       
       response2.StatusCode.Should().Be(HttpStatusCode.OK);
       (await response2.Content.ReadFromJsonAsync<ArticleResponse>())?.ArticleId.Should().Be(secondArticleId);
    }
    
    [Fact]
    public async Task ArticleWontBeReturnedIfItDoesNotExist()
    {
        // Arrange
        var client = factory.CreateClient();
        const string articleId = "article-id";
       
        // Act
        var response = await client.GetAsync($"/api/articles/{articleId}");
       
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task DraftWontBeReturnedIfYouHaveNoCanManageArticlesClaim()
    {
        // Arrange
        var client = factory.CreateClient();
        var customClient = factory.AuthorizeAs(UserTypes.Admin).CreateClient();
        const string articleId = "article-id";
        
        await customClient.PostAsJsonAsync(
            requestUri: "/api/articles", 
            value: DataGenerator.Article.GetCreateRequest() with { ArticleId = articleId, IsPublished = false });
       
        // Act
        var response = await client.GetAsync($"/api/articles/{articleId}");
       
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}