namespace WebAPI.IntegrationTests.CollectionsControllerEndpoints;

[Collection("Tests")]
public sealed class UpdateCollectionTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
    private readonly CustomWebApplicationFactory _factory = factory;

    [Fact]
    public async Task CollectionCanBeUpdated()
    {
        // Arrange
        var customClient = _factory.AuthorizeAs(UserTypes.Admin).CreateClient();
        const string defaultTitle = "Default title";
        const string updatedTitle = "Updated title";
        
        const string collectionId = "collection-id";
        await customClient.PostAsJsonAsync(
            requestUri: "/api/collections",
            value: DataGenerator.Collection.GetCreateCollectionRequest() with
            {
                CollectionId = collectionId, 
                Title = defaultTitle
            });
        
        // Act
        var getCollectionBeforeUpdateResponse = await customClient.GetAsync($"/api/collections/{collectionId}");
        
        var updateCollectionResponse = await customClient.PutAsJsonAsync(
            requestUri: "/api/collections",
            value: DataGenerator.Collection.GetUpdateCollectionRequest() with
            {
                CollectionId = collectionId, 
                Title = updatedTitle
            });
        
        var getCollectionAfterUpdateResponse = await customClient.GetAsync($"/api/collections/{collectionId}");
        
        // Assert
        updateCollectionResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        var firstContent = await getCollectionBeforeUpdateResponse.Content.ReadFromJsonAsync<ArticleResponse>();
        var secondContent = await getCollectionAfterUpdateResponse.Content.ReadFromJsonAsync<ArticleResponse>();
        firstContent?.Title.Should().NotBeSameAs(secondContent?.Title);
    }
    
    [Fact]
    public async Task CollectionCannotBeUpdatedIfItDoesNotExist()
    {
        // Arrange
        var customClient = _factory.AuthorizeAs(UserTypes.Admin).CreateClient();
        const string collectionId = "collection-id";
        
        // Act
        var response = await customClient.PutAsJsonAsync(
            requestUri: "/api/collections",
            value: DataGenerator.Collection.GetUpdateCollectionRequest() with { CollectionId = collectionId });
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task CollectionCannotBeUpdatedWithInvalidData()
    {
        // Arrange
        var customClient = _factory.AuthorizeAs(UserTypes.Admin).CreateClient();
        const string defaultTitle = "Default title";
        const string tooShortTitle = "Aa";
        
        const string collectionId = "collection-id";
        await customClient.PostAsJsonAsync(
            requestUri: "/api/collections",
            value: DataGenerator.Collection.GetCreateCollectionRequest() with
            {
                CollectionId = collectionId, 
                Title = defaultTitle
            });
        
        // Act
        var response = await customClient.PutAsJsonAsync(
            requestUri: "/api/collections",
            value: DataGenerator.Collection.GetUpdateCollectionRequest() with
            {
                CollectionId = collectionId,
                Title = tooShortTitle
            });
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task OnlyAuthorizedUserCanUpdateCollection()
    {
        // Arrange
        var client = _factory.CreateClient();
        const string collectionId = "collection-id";
        
        // Act
        var response = await client.PutAsJsonAsync(
            requestUri: "/api/collections",
            value: DataGenerator.Collection.GetUpdateCollectionRequest() with { CollectionId = collectionId });
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task OnlyUserWithCanManageArticlesClaimCanUpdateCollection()
    {
        // Arrange
        var customClient = _factory.AuthorizeAs(UserTypes.User).CreateClient();
        const string collectionId = "collection-id";
        
        // Act
        var response = await customClient.PutAsJsonAsync(
            requestUri: "/api/collections",
            value: DataGenerator.Collection.GetUpdateCollectionRequest() with { CollectionId = collectionId });
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}