namespace WebAPI.IntegrationTests.ArticlesControllerEndpoints;

[Collection("Tests")]
public sealed class GetArticlesTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
    private readonly CustomWebApplicationFactory _factory = factory;
    
    [Fact]
    public async Task EmptyListWillBeReturnedIfNoArticlesExist()
    {
        // Arrange
        var customClient = _factory.AuthorizeAs(UserTypes.Admin).CreateClient();
        const int numberOfArticles = 0;

        // Act
        var response = await customClient.GetAsync("/api/articles");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Headers.GetValues("X-Total-Count").Single().Should().Be($"{numberOfArticles}");
        (await response.Content.ReadFromJsonAsync<List<ArticlePreviewResponse>>()).Should().BeEmpty();
    }
    
    [Fact]
    public async Task ArticlesWillBeReturnedIfArticlesExist()
    {
        // Arrange
        var customClient = _factory.AuthorizeAs(UserTypes.Admin).CreateClient();
        const int numberOfArticles = 2;
        
        for (var index = 1; index <= numberOfArticles; index++)
            await customClient.PostAsJsonAsync(
                requestUri: "/api/articles", 
                value: DataGenerator.Article.GetCreateRequest() with { ArticleId = $"article-{index}" });
        
        // Act
        var response = await customClient.GetAsync("/api/articles");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Headers.GetValues("X-Total-Count").Single().Should().Be($"{numberOfArticles}");
        (await response.Content.ReadFromJsonAsync<List<ArticlePreviewResponse>>()).Should().HaveCount(numberOfArticles);
    }
    
    [Fact]
    public async Task OnlyAuthorizedUserCanReadArticles()
    {
        // Arrange
        var client = _factory.CreateClient();
        
        // Act
        var response = await client.GetAsync("/api/articles");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task OnlyUserWithCanManageArticlesClaimCanReadArticles()
    {
        // Arrange
        var customClient = _factory.AuthorizeAs(UserTypes.User).CreateClient();
        
        // Act
        var response = await customClient.GetAsync("/api/articles");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
    
    [Fact]
    public async Task PaginationShouldWork()
    {
        // Arrange
        var customClient = _factory.AuthorizeAs(UserTypes.Admin).CreateClient();
        const int numberOfDrafts = 15;
        const int numberOfPublishedArticles = 1;
        var publishedArticleId = $"article-{numberOfDrafts + numberOfPublishedArticles}";

        for (var index = 1; index <= numberOfDrafts; index++)
            await customClient.PostAsJsonAsync(
                requestUri: "/api/articles",
                value: DataGenerator.Article.GetCreateRequest() with
                {
                    ArticleId = $"article-{index}",
                    IsPublished = false
                });
        
        await customClient.PostAsJsonAsync(
            requestUri: "/api/articles",
            value: DataGenerator.Article.GetCreateRequest() with
            {
                ArticleId = publishedArticleId,
                IsPublished = true
            });
        
        // Act
        var response1 = await customClient.GetAsync("/api/articles");
        var response2 = await customClient.GetAsync("/api/articles?page=1&size=5");
        var response3 = await customClient.GetAsync("/api/articles?page=2&size=10");
        var response4 = await customClient.GetAsync("/api/articles?page=3&size=10");
        List<HttpResponseMessage> responses = [response1, response2, response3, response4];
        
        // Assert
        responses.ForEach(response => response.StatusCode.Should().Be(HttpStatusCode.OK));
        responses.ForEach(response => 
            response.Headers.GetValues("X-Total-Count")
                .Single().Should().Be($"{numberOfDrafts + numberOfPublishedArticles}"));
        
        (await response1.Content.ReadFromJsonAsync<List<ArticlePreviewResponse>>()).Should().HaveCount(10);
        (await response2.Content.ReadFromJsonAsync<List<ArticlePreviewResponse>>()).Should().HaveCount(5);
        (await response3.Content.ReadFromJsonAsync<List<ArticlePreviewResponse>>()).Should().HaveCount(6);
        (await response4.Content.ReadFromJsonAsync<List<ArticlePreviewResponse>>()).Should().HaveCount(0);
    }
}