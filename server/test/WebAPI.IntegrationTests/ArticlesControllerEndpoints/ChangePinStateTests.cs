namespace WebAPI.IntegrationTests.ArticlesControllerEndpoints;

[Collection("Tests")]
public sealed class ChangePinStateTests(CustomWebApplicationFactory factory) : IAsyncLifetime
{
    public Task InitializeAsync() => Task.CompletedTask;
    
    public async Task DisposeAsync() => await factory.ResetDatabaseAsync();

    [Fact]
    public async Task PinStateCanBeChanged()
    {
        // Arrange
        var customClient = factory.AuthorizeAs(UserTypes.Admin).CreateClient();

        const string firstArticleId = "article-1";
        await customClient.PostAsJsonAsync(
            requestUri: "/api/articles", 
            value: DataGenerator.Article.GetCreateRequest() with { ArticleId = firstArticleId, IsPinned = false });
        
        const string secondArticleId = "article-2";
        await customClient.PostAsJsonAsync(
            requestUri: "/api/articles", 
            value: DataGenerator.Article.GetCreateRequest() with { ArticleId = secondArticleId, IsPinned = true });
        
        // Act
        var changePinStateResponse1 = await customClient.PatchAsync($"/api/articles/{firstArticleId}/pin", null);
        var getArticleResponse1 = await customClient.GetAsync($"/api/articles/{firstArticleId}");
        
        var changePinStateResponse2 = await customClient.PatchAsync($"/api/articles/{secondArticleId}/pin", null);
        var getArticleResponse2 = await customClient.GetAsync($"/api/articles/{secondArticleId}");

        // Assert
        changePinStateResponse1.StatusCode.Should().Be(HttpStatusCode.NoContent);
        (await getArticleResponse1.Content.ReadFromJsonAsync<ArticleResponse>())?.IsPinned.Should().BeTrue();
        
        changePinStateResponse2.StatusCode.Should().Be(HttpStatusCode.NoContent);
        (await getArticleResponse2.Content.ReadFromJsonAsync<ArticleResponse>())?.IsPinned.Should().BeFalse();
    }
    
    [Fact]
    public async Task PinStateCannotBeChangedIfArticleDoesNotExist()
    {
        // Arrange
        var customClient = factory.AuthorizeAs(UserTypes.Admin).CreateClient();
        const string articleId = "article-id";
        
        // Act
        var response = await customClient.PatchAsync($"/api/articles/{articleId}/pin", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task OnlyAuthorizedUserCanChangePinState()
    {
        // Arrange
        var client = factory.CreateClient();
        const string articleId = "article-id";
        
        // Act
        var response = await client.PatchAsync($"/api/articles/{articleId}/pin", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task OnlyUserWithCanManageArticlesClaimCanChangePinState()
    {
        // Arrange
        var customClient = factory.AuthorizeAs(UserTypes.User).CreateClient();
        const string articleId = "article-id";
        
        // Act
        var response = await customClient.PatchAsync($"/api/articles/{articleId}/pin", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}