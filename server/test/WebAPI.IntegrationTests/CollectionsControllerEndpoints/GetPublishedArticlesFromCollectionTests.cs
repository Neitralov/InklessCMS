namespace WebAPI.IntegrationTests.CollectionsControllerEndpoints;

[Collection("Tests")]
public sealed class GetPublishedArticlesFromCollectionTests(CustomWebApplicationFactory factory)
    : BaseIntegrationTest(factory)
{
    private readonly CustomWebApplicationFactory _factory = factory;

    [Fact]
    public async Task EmptyListWillBeReturnedIfCollectionDoesNotContainPublishedArticles()
    {
        // Arrange
        var customClient = _factory.AuthorizeAs(UserTypes.Admin).CreateClient();
        const string collectionId = "collection-id";

        await customClient.PostAsJsonAsync(
            requestUri: "/api/collections",
            value: DataGenerator.Collection.GetCreateCollectionRequest() with { CollectionId = collectionId });

        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync($"/api/collections/{collectionId}/published");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        (await response.Content.ReadFromJsonAsync<List<ArticlePreviewResponse>>()).Should().BeEmpty();
    }

    [Fact]
    public async Task PublishedArticlesWillBeReturnedIfCollectionContainsPublishedArticles()
    {
        // Arrange
        var customClient = _factory.AuthorizeAs(UserTypes.Admin).CreateClient();
        const string collectionId = "collection-id";
        const int numberOfPublishedArticles = 1;

        await customClient.PostAsJsonAsync(
            requestUri: "/api/collections",
            value: DataGenerator.Collection.GetCreateCollectionRequest() with { CollectionId = collectionId });

        const string firstArticleId = "article-1";
        await customClient.PostAsJsonAsync(
            requestUri: "/api/articles",
            value: DataGenerator.Article.GetCreateRequest() with { ArticleId = firstArticleId, IsPublished = true });
        await customClient.PostAsJsonAsync(
            requestUri: $"/api/collections/{collectionId}",
            value: DataGenerator.Collection.GetAddArticleToCollectionRequest() with { ArticleId = firstArticleId });

        const string secondArticleId = "article-2";
        await customClient.PostAsJsonAsync(
            requestUri: "/api/articles",
            value: DataGenerator.Article.GetCreateRequest() with { ArticleId = secondArticleId, IsPublished = false });
        await customClient.PostAsJsonAsync(
            requestUri: $"/api/collections/{collectionId}",
            value: DataGenerator.Collection.GetAddArticleToCollectionRequest() with { ArticleId = secondArticleId });

        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync($"/api/collections/{collectionId}/published");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        (await response.Content.ReadFromJsonAsync<List<ArticlePreviewResponse>>())
            .Should().HaveCount(numberOfPublishedArticles);
    }

    [Fact]
    public async Task PublishedArticlesWontBeReturnedIfCollectionDoesNotExist()
    {
        // Arrange
        var client = _factory.CreateClient();
        const string collectionId = "collection-id";

        // Act
        var response = await client.GetAsync($"/api/collections/{collectionId}/published");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task PaginationShouldWork()
    {
        // Arrange
        var customClient = _factory.AuthorizeAs(UserTypes.Admin).CreateClient();
        const string collectionId = "collection-id";

        await customClient.PostAsJsonAsync(
            requestUri: "/api/collections",
            value: DataGenerator.Collection.GetCreateCollectionRequest() with { CollectionId = collectionId });

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
        for (var index = 1; index <= numberOfPublishedArticles; index++)
            await customClient.PostAsJsonAsync(
                requestUri: $"/api/collections/{collectionId}",
                value: DataGenerator.Collection.GetAddArticleToCollectionRequest() with
                {
                    ArticleId = $"article-{index}"
                });

        await customClient.PostAsJsonAsync(
            requestUri: "/api/articles",
            value: DataGenerator.Article.GetCreateRequest() with { ArticleId = draftArticleId, IsPublished = false });
        await customClient.PostAsJsonAsync(
            requestUri: $"/api/collections/{collectionId}",
            value: DataGenerator.Collection.GetAddArticleToCollectionRequest() with { ArticleId = draftArticleId });

        var client = _factory.CreateClient();

        // Act
        var response1 = await client.GetAsync($"/api/collections/{collectionId}/published");
        var response2 = await client.GetAsync($"/api/collections/{collectionId}/published?page=1&size=5");
        var response3 = await client.GetAsync($"/api/collections/{collectionId}/published?page=2&size=10");
        var response4 = await client.GetAsync($"/api/collections/{collectionId}/published?page=3&size=10");
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
