namespace WebAPI.IntegrationTests.CollectionsControllerEndpoints;

[Collection("Tests")]
public sealed class DeleteCollectionTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
    private readonly CustomWebApplicationFactory _factory = factory;
    
    [Fact]
    public async Task CollectionCanBeDeleted()
    {
        // Arrange
        var customClient = _factory.AuthorizeAs(UserTypes.Admin).CreateClient();
        
        const string collectionId = "collection-id";
        await customClient.PostAsJsonAsync(
            requestUri: "/api/collections",
            value: DataGenerator.Collection.GetCreateCollectionRequest() with { CollectionId = collectionId });
        
        // Act
        var deleteCollectionResponse = await customClient.DeleteAsync($"/api/collections/{collectionId}");
        var getCollectionResponse = await customClient.GetAsync($"/api/collections/{collectionId}");
        
        // Assert
        deleteCollectionResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        getCollectionResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task CollectionCannotBeDeletedIfItDoesNotExist()
    {
        // Arrange
        var customClient = _factory.AuthorizeAs(UserTypes.Admin).CreateClient();
        const string collectionId = "collection-id";
        
        // Act
        var response = await customClient.DeleteAsync($"/api/collections/{collectionId}");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task CascadingDeletionOfArticlesWontOccurIfDeleteCollectionWithArticles()
    {
        // Arrange
        var customClient = _factory.AuthorizeAs(UserTypes.Admin).CreateClient();
        
        const string collectionId = "collection-id"; 
        await customClient.PostAsJsonAsync(
            requestUri: "/api/collections",
            value: DataGenerator.Collection.GetCreateCollectionRequest() with { CollectionId = collectionId });
        
        const string articleId = "article-id";
        await customClient.PostAsJsonAsync(
            requestUri: "/api/articles", 
            value: DataGenerator.Article.GetCreateRequest() with { ArticleId = articleId });
        await customClient.PostAsJsonAsync(
            requestUri: $"/api/collections/{collectionId}",
            value: DataGenerator.Collection.GetAddArticleToCollectionRequest() with { ArticleId = articleId });
        
        // Act
        var deleteCollectionResponse = await customClient.DeleteAsync($"/api/collections/{collectionId}");
        var getCollectionResponse = await customClient.GetAsync($"/api/collections/{collectionId}");
        var getArticleResponse = await customClient.GetAsync($"/api/articles/{articleId}");
        
        // Assert
        deleteCollectionResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        getCollectionResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        getArticleResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task OnlyAuthorizedUserCanDeleteCollection()
    {
        // Arrange
        var client = _factory.CreateClient();
        const string collectionId = "collection-id"; 
        
        // Act
        var response = await client.DeleteAsync($"/api/collections/{collectionId}");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task OnlyUserWithCanManageArticlesClaimCanDeleteCollection()
    {
        // Arrange
        var customClient = _factory.AuthorizeAs(UserTypes.User).CreateClient();
        const string collectionId = "collection-id"; 
        
        // Act
        var response = await customClient.DeleteAsync($"/api/collections/{collectionId}");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}