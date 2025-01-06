namespace WebAPI.IntegrationTests.ArticlesControllerEndpoints;

[Collection("Tests")]
public sealed class DeleteArticleTests(CustomWebApplicationFactory factory) : IAsyncLifetime
{
    public Task InitializeAsync() => Task.CompletedTask;
    
    public async Task DisposeAsync() => await factory.ResetDatabaseAsync();

    [Fact]
    public async Task ArticleCanBeDeleted()
    {
        // Arrange
        var customClient = factory.AuthorizeAs(UserTypes.Admin).CreateClient();
        const string articleId = "article-id";
        
        await customClient.PostAsJsonAsync(
            requestUri: "/api/articles", 
            value: DataGenerator.Article.GetCreateRequest() with { ArticleId = articleId });
       
        // Act
        var deleteArticleResponse = await customClient.DeleteAsync($"/api/articles/{articleId}");
        var getArticleResponse = await customClient.GetAsync($"/api/articles/{articleId}");
       
        // Assert
        deleteArticleResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        getArticleResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task ArticleCannotBeDeletedIfItDoesNotExist()
    {
        // Arrange
        var customClient = factory.AuthorizeAs(UserTypes.Admin).CreateClient();
        const string articleId = "article-id";
       
        // Act
        var response = await customClient.DeleteAsync($"/api/articles/{articleId}");
       
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task OnlyAuthorizedUserCanDeleteArticle()
    {
        // Arrange
        var client = factory.CreateClient();
        const string articleId = "article-id";
        
        // Act
        var response = await client.DeleteAsync($"/api/articles/{articleId}");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task OnlyUserWithCanManageArticlesClaimCanDeleteArticle()
    {
        // Arrange
        var customClient = factory.AuthorizeAs(UserTypes.User).CreateClient();
        const string articleId = "article-id";
        
        // Act
        var response = await customClient.DeleteAsync($"/api/articles/{articleId}");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}