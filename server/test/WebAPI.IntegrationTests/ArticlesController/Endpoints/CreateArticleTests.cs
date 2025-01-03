using Xunit.Abstractions;

namespace WebAPI.IntegrationTests.ArticlesController.Endpoints;

[Collection("Tests")]
public sealed class CreateArticleTests(CustomWebApplicationFactory factory, ITestOutputHelper testOutputHelper) 
    : ArticlesControllerTests, IAsyncLifetime
{
    public Task InitializeAsync() => Task.CompletedTask;
    
    public async Task DisposeAsync() => await factory.ResetDatabaseAsync();
    
    [Fact]
    public async Task ArticleCanBeCreated()
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
        var request = GenerateCreateArticleRequest();
        
        // Act
        var response = await client.PostAsJsonAsync("/api/articles", request);
        testOutputHelper.WriteLine(await response.Content.ReadAsStringAsync());
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        (await response.Content.ReadFromJsonAsync<ArticleResponse>()).Should().NotBeNull();
    }
    
    [Fact]
    public async Task InvalidArticleCannotBeCreated()
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
        var request = GenerateCreateArticleRequest() with { ArticleId = "Inv@lid-Id" };
        
        // Act
        var response = await client.PostAsJsonAsync("/api/articles", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task OnlyAuthorizedUserCanCreateArticle()
    {
        // Arrange
        var client = factory.CreateClient();
        var request = GenerateCreateArticleRequest();
        
        // Act
        var response = await client.PostAsJsonAsync("/api/articles", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task OnlyUserWithCanManageArticlesClaimCanCreateArticle()
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
        var request = GenerateCreateArticleRequest();
        
        // Act
        var response = await client.PostAsJsonAsync("/api/articles", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}