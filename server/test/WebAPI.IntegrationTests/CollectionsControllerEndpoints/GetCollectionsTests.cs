namespace WebAPI.IntegrationTests.CollectionsControllerEndpoints;

[Collection("Tests")]
public sealed class GetCollectionsTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
    private readonly CustomWebApplicationFactory _factory = factory;
    
    [Fact]
    public async Task EmptyListWillBeReturnedIfNoCollectionsExist()
    {
        // Arrange
        var client = _factory.CreateClient();
        
        // Act
        var response = await client.GetAsync("/api/collections");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        (await response.Content.ReadFromJsonAsync<List<CollectionPreviewResponse>>()).Should().BeEmpty();
    }
    
    [Fact]
    public async Task CollectionsWillBeReturnedIfCollectionsExist()
    {
        // Arrange
        var customClient = _factory.AuthorizeAs(UserTypes.Admin).CreateClient();
        const int numberOfCollections = 2;

        for (var index = 1; index <= numberOfCollections; index++)
            await customClient.PostAsJsonAsync(
                requestUri: "/api/collections",
                value: DataGenerator.Collection.GetCreateCollectionRequest() with
                {
                    CollectionId = $"collection-{index}"
                });
        
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/collections");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        (await response.Content.ReadFromJsonAsync<List<CollectionPreviewResponse>>())
            .Should().HaveCount(numberOfCollections);
    }
}