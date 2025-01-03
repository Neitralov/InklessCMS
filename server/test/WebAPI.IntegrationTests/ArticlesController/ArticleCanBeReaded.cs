namespace WebAPI.IntegrationTests.ArticlesController;

public sealed class ArticleIntegrationTests
{
    // [Fact]
    // public async Task ArticleCanBeCreatedAndReaded()
    // {
    //     // Arrange
    //     var customFactory = _factory.WithWebHostBuilder(builder =>
    //     {
    //         builder.ConfigureTestServices(services =>
    //         {
    //             services.AdminLogIn();
    //         });
    //     });
    //     var client = customFactory.CreateClient();
    //     
    //     const string articleId = "article-id";
    //     var request = GenerateCreateArticleRequest() with { ArticleId = articleId };
    //     
    //     // Act
    //     await client.PostAsJsonAsync("/api/articles", request);
    //     var response = await client.GetAsync($"/api/articles/{articleId}");
    //     
    //     // Assert
    //     response.EnsureSuccessStatusCode();
    //     var content = await response.Content.ReadFromJsonAsync<ArticleResponse>();
    //     content.Should().NotBeNull();
    // }
}