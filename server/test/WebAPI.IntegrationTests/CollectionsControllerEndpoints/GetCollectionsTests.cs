namespace WebAPI.IntegrationTests.CollectionsControllerEndpoints;

[Collection("Tests")]
public sealed class GetCollectionsTests(CustomWebApplicationFactory factory) : IAsyncLifetime
{
    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync() => await factory.ResetDatabaseAsync();
    
    [Fact]
    public async Task EmptyListWillBeReturnedIfNoCollectionsExist()
    {
        // Arrange
        var client = factory.CreateClient();
        
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
        var customClient = factory.AuthorizeAs(UserTypes.Admin).CreateClient();
        const int numberOfCollections = 2;

        for (var index = 1; index <= numberOfCollections; index++)
            await customClient.PostAsJsonAsync(
                requestUri: "/api/collections",
                value: DataGenerator.Collection.GetCreateCollectionRequest() with
                {
                    CollectionId = $"collection-{index}"
                });
        
        var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/collections");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        (await response.Content.ReadFromJsonAsync<List<CollectionPreviewResponse>>())
            .Should().HaveCount(numberOfCollections);
    }
}