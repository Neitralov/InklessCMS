namespace WebAPI.IntegrationTests.ArticlesControllerEndpoints;

[Collection("Tests")]
public sealed class DeleteArticleTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
    private readonly CustomWebApplicationFactory _factory = factory;

    [Fact]
    public async Task ArticleCanBeDeleted()
    {
        // Arrange
        var customClient = _factory.AuthorizeAs(UserTypes.Admin).CreateClient();
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
        var customClient = _factory.AuthorizeAs(UserTypes.Admin).CreateClient();
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
        var client = _factory.CreateClient();
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
        var customClient = _factory.AuthorizeAs(UserTypes.User).CreateClient();
        const string articleId = "article-id";
        
        // Act
        var response = await customClient.DeleteAsync($"/api/articles/{articleId}");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}