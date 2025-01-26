namespace WebAPI.IntegrationTests.ArticlesControllerEndpoints;

[Collection("Tests")]
public sealed class GetPublishedArticlesTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
    private readonly CustomWebApplicationFactory _factory = factory;
    
    [Fact]
    public async Task EmptyListWillBeReturnedIfNoPublishedArticlesExist()
    {
        // Arrange
        var client = _factory.CreateClient();
        var customClient = _factory.AuthorizeAs(UserTypes.Admin).CreateClient();
        const int numberOfArticles = 0;
        
        await customClient.PostAsJsonAsync(
            requestUri: "/api/articles", 
            value: DataGenerator.Article.GetCreateRequest() with { IsPublished = false });
        
        // Act
        var response = await client.GetAsync("/api/articles/published");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Headers.GetValues("X-Total-Count").Single().Should().Be($"{numberOfArticles}");
        (await response.Content.ReadFromJsonAsync<List<ArticlePreviewResponse>>()).Should().BeEmpty();
    }
    
    [Fact]
    public async Task PublishedArticlesWillBeReturnedIfPublishedArticlesExist()
    {
        // Arrange
        var client = _factory.CreateClient();
        var customClient = _factory.AuthorizeAs(UserTypes.Admin).CreateClient();
        const int numberOfPublishedArticles = 1;

        const string firstArticleId = "article-1";
        await customClient.PostAsJsonAsync(
            requestUri: "/api/articles", 
            value: DataGenerator.Article.GetCreateRequest() with { ArticleId = firstArticleId, IsPublished = false });
        
        const string secondArticleId = "article-2";
        await customClient.PostAsJsonAsync(
            requestUri: "/api/articles", 
            value: DataGenerator.Article.GetCreateRequest() with { ArticleId = secondArticleId, IsPublished = true });
        
        // Act
        var response = await client.GetAsync("/api/articles/published");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Headers.GetValues("X-Total-Count").Single().Should().Be($"{numberOfPublishedArticles}");
        var content = await response.Content.ReadFromJsonAsync<List<ArticlePreviewResponse>>();
        content.Should().HaveCount(numberOfPublishedArticles);
    }

    [Fact]
    public async Task PaginationShouldWork()
    {
        // Arrange
        var client = _factory.CreateClient();
        var customClient = _factory.AuthorizeAs(UserTypes.Admin).CreateClient();
        const int numberOfPublishedArticles = 15;
        const int numberOfDrafts = 1;
        var draftArticleId = $"article-{numberOfPublishedArticles + numberOfDrafts}";
        
        for (var index = 1; index <= numberOfPublishedArticles; index++)
            await customClient.PostAsJsonAsync(
                requestUri: "/api/articles",
                value: DataGenerator.Article.GetCreateRequest() with
                {
                    ArticleId = $"article-{index}",
                    IsPublished = true
                });
        
        await customClient.PostAsJsonAsync(
            requestUri: "/api/articles", 
            value: DataGenerator.Article.GetCreateRequest() with { ArticleId = draftArticleId, IsPublished = false });
        
        // Act
        var response1 = await client.GetAsync("/api/articles/published");
        var response2 = await client.GetAsync("/api/articles/published?page=1&size=5");
        var response3 = await client.GetAsync("/api/articles/published?page=2&size=10");
        var response4 = await client.GetAsync("/api/articles/published?page=3&size=10");
        List<HttpResponseMessage> responses = [response1, response2, response3, response4];
        
        // Assert
        responses.ForEach(response => response.StatusCode.Should().Be(HttpStatusCode.OK));
        responses.ForEach(response => 
            response.Headers.GetValues("X-Total-Count").Single().Should().Be($"{numberOfPublishedArticles}"));
        
        (await response1.Content.ReadFromJsonAsync<List<ArticlePreviewResponse>>()).Should().HaveCount(10);
        (await response2.Content.ReadFromJsonAsync<List<ArticlePreviewResponse>>()).Should().HaveCount(5);
        (await response3.Content.ReadFromJsonAsync<List<ArticlePreviewResponse>>()).Should().HaveCount(5);
        (await response4.Content.ReadFromJsonAsync<List<ArticlePreviewResponse>>()).Should().HaveCount(0);
    }
}