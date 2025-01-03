namespace WebAPI.IntegrationTests.ArticlesController.Endpoints;

[Collection("Tests")]
public sealed class GetArticlesTests(CustomWebApplicationFactory factory) : ArticlesControllerTests, IAsyncLifetime
{
    public Task InitializeAsync() => Task.CompletedTask;
    
    public async Task DisposeAsync() => await factory.ResetDatabaseAsync();
    
    [Fact]
    public async Task EmptyListWillBeReturnedIfNoArticlesExist()
    {
        // Arrange
        var customFactory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AdminLogIn();
            });
        });
        
        var client = customFactory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/articles");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        (await response.Content.ReadFromJsonAsync<List<ArticlePreviewResponse>>()).Should().BeEmpty();
    }
    
    [Fact]
    public async Task ArticlesWillBeReturnedIfArticlesExists()
    {
        // Arrange
        var customFactory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AdminLogIn();
            });
        });
        
        var client = customFactory.CreateClient();

        const int numberOfArticles = 2;
        foreach (var createArticleRequest in GenerateCreateArticleRequests(count: numberOfArticles))
            await client.PostAsJsonAsync("/api/articles", createArticleRequest);
        
        // Act
        var response = await client.GetAsync("/api/articles");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        (await response.Content.ReadFromJsonAsync<List<ArticlePreviewResponse>>()).Should().HaveCount(numberOfArticles);
    }
    
    [Fact]
    public async Task OnlyAuthorizedUserCanReadArticles()
    {
        // Arrange
        var client = factory.CreateClient();
        
        // Act
        var response = await client.GetAsync("/api/articles");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task OnlyUserWithCanManageArticlesClaimCanReadArticles()
    {
        // Arrange
        var customFactory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.UserLogIn();
            });
        });
        
        var client = customFactory.CreateClient();
        
        // Act
        var response = await client.GetAsync("/api/articles");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}