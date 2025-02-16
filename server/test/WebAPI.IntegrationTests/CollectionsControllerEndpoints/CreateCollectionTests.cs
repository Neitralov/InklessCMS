namespace WebAPI.IntegrationTests.CollectionsControllerEndpoints;

[Collection("Tests")]
public sealed class CreateCollectionTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
    private readonly CustomWebApplicationFactory _factory = factory;

    [Fact]
    public async Task CollectionCanBeCreated()
    {
        // Arrange
        var customClient = _factory.AuthorizeAs(UserTypes.Admin).CreateClient();
        const string collectionId = "collection-id";

        // Act
        var response = await customClient.PostAsJsonAsync(
            requestUri: "/api/collections",
            value: DataGenerator.Collection.GetCreateCollectionRequest() with { CollectionId = collectionId });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        (await response.Content.ReadFromJsonAsync<CollectionPreviewResponse>())?.CollectionId.Should().Be(collectionId);
    }

    [Fact]
    public async Task InvalidCollectionCannotBeCreated()
    {
        // Arrange
        var customClient = _factory.AuthorizeAs(UserTypes.Admin).CreateClient();
        const string collectionId = "inv@lid-id";

        // Act
        var response = await customClient.PostAsJsonAsync(
            requestUri: "/api/collections",
            value: DataGenerator.Collection.GetCreateCollectionRequest() with { CollectionId = collectionId });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task OnlyAuthorizedUserCanCreateCollection()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync(
            requestUri: "/api/collections",
            value: DataGenerator.Collection.GetCreateCollectionRequest());

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task OnlyUserWithCanManageArticlesClaimCanCreateCollection()
    {
        // Arrange
        var customClient = _factory.AuthorizeAs(UserTypes.User).CreateClient();

        // Act
        var response = await customClient.PostAsJsonAsync(
            requestUri: "/api/collections",
            value: DataGenerator.Collection.GetCreateCollectionRequest());

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
