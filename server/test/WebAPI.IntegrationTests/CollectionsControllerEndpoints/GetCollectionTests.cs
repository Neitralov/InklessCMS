namespace WebAPI.IntegrationTests.CollectionsControllerEndpoints;

[Collection("Tests")]
public sealed class GetCollectionTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
    private readonly CustomWebApplicationFactory _factory = factory;
    
    [Fact]
    public async Task EmptyListWillBeReturnedIfCollectionIsEmpty()
    {
        // Arrange
        var customClient = _factory.AuthorizeAs(UserTypes.Admin).CreateClient();
        const string collectionId = "collection-id"; 
        
        await customClient.PostAsJsonAsync(
            requestUri: "/api/collections",
            value: DataGenerator.Collection.GetCreateCollectionRequest() with { CollectionId = collectionId });

        // Act
        var response = await customClient.GetAsync($"/api/collections/{collectionId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        (await response.Content.ReadFromJsonAsync<CollectionResponse>())?.Articles.Should().BeEmpty();
    }
    
    [Fact]
    public async Task ArticlesWillBeReturnedIfCollectionContainsArticles()
    {
        // Arrange
        var customClient = _factory.AuthorizeAs(UserTypes.Admin).CreateClient();
        const string collectionId = "collection-id"; 
        const int numberOfArticles = 2;
        
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
        
        // Act
        var response = await customClient.GetAsync($"/api/collections/{collectionId}");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        (await response.Content.ReadFromJsonAsync<CollectionResponse>())?.Articles.Should().HaveCount(numberOfArticles);
    }
    
    [Fact]
    public async Task ArticlesWontBeReturnedIfCollectionDoesNotExist()
    {
        // Arrange
        var customClient = _factory.AuthorizeAs(UserTypes.Admin).CreateClient();
        const string collectionId = "collection-id";
        
        // Act
        var response = await customClient.GetAsync($"/api/collections/{collectionId}");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task OnlyAuthorizedUserCanGetCollection()
    {
        // Arrange
        var client = _factory.CreateClient();
        const string collectionId = "collection-id"; 
        
        // Act
        var response = await client.GetAsync($"/api/collections/{collectionId}");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task OnlyUserWithCanManageArticlesClaimCanGetCollection()
    {
        // Arrange
        var customClient = _factory.AuthorizeAs(UserTypes.User).CreateClient();
        const string collectionId = "collection-id"; 
        
        // Act
        var response = await customClient.GetAsync($"/api/collections/{collectionId}");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}