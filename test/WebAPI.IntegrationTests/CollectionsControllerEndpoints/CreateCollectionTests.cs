namespace WebAPI.IntegrationTests.CollectionsControllerEndpoints;

[Collection("Tests")]
public sealed class CreateCollectionTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
    private readonly CustomWebApplicationFactory _factory = factory;

    [Fact]
    public async Task CollectionCanBeCreated()
    {
        // Arrange
        var gqlClient = _factory.AuthorizeAs(UserTypes.Admin).CreateClient().ToGqlClient();
        const string collectionId = "collection-id";

        // Act
        var gqlResponse = await gqlClient.CreateCollection(
            Requests.Collection.CollectionInput with { CollectionId = collectionId });

        // Assert
        gqlResponse.CollectionId.ShouldBe(collectionId);
    }

    [Fact]
    public async Task InvalidCollectionCannotBeCreated()
    {
        // Arrange
        var gqlClient = _factory.AuthorizeAs(UserTypes.Admin).CreateClient().ToGqlClient();
        const string collectionId = "inv@lid-id";

        // Act
        var exception = await Should.ThrowAsync<GraphQLHttpRequestException>(async () =>
        {
            await gqlClient.CreateCollection(Requests.Collection.CollectionInput with { CollectionId = collectionId });
        });
        
        // Assert
        exception.Content!.ShouldContain(Collection.Errors.InvalidId.Code);
    }

    [Fact]
    public async Task OnlyAuthorizedUserCanCreateCollection()
    {
        // Arrange
        var gqlClient = _factory.CreateClient().ToGqlClient();

        // Act
        var exception = await Should.ThrowAsync<GraphQLHttpRequestException>(async () =>
        {
            await gqlClient.CreateCollection(Requests.Collection.CollectionInput);
        });

        // Assert
        exception.Content!.ShouldContain("AUTH_NOT_AUTHORIZED");
    }

    [Fact]
    public async Task OnlyUserWithCanManageArticlesClaimCanCreateCollection()
    {
        // Arrange
        var gqlClient = _factory.AuthorizeAs(UserTypes.User).CreateClient().ToGqlClient();

        // Act
        var exception = await Should.ThrowAsync<GraphQLHttpRequestException>(async () =>
        {
            await gqlClient.CreateCollection(Requests.Collection.CollectionInput);
        });

        // Assert
        exception.Content!.ShouldContain("AUTH_NOT_AUTHORIZED");
    }
}
