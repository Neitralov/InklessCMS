namespace WebAPI.IntegrationTests.CollectionsTests;

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
            Inputs.Collection.CollectionInput with { CollectionId = collectionId });

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
        var exception = await Should.ThrowAsync<GraphQLException>(async () =>
        {
            await gqlClient.CreateCollection(Inputs.Collection.CollectionInput with { CollectionId = collectionId });
        });
        
        // Assert
        exception.Message!.ShouldContain(Collection.Errors.InvalidId.Code);
    }

    [Fact]
    public async Task OnlyAuthorizedUserCanCreateCollection()
    {
        // Arrange
        var gqlClient = _factory.CreateClient().ToGqlClient();

        // Act
        var exception = await Should.ThrowAsync<GraphQLException>(async () =>
        {
            await gqlClient.CreateCollection(Inputs.Collection.CollectionInput);
        });

        // Assert
        exception.Message!.ShouldContain("The current user is not authorized to access this resource.");
    }

    [Fact]
    public async Task OnlyUserWithCanManageArticlesClaimCanCreateCollection()
    {
        // Arrange
        var gqlClient = _factory.AuthorizeAs(UserTypes.User).CreateClient().ToGqlClient();

        // Act
        var exception = await Should.ThrowAsync<GraphQLException>(async () =>
        {
            await gqlClient.CreateCollection(Inputs.Collection.CollectionInput);
        });

        // Assert
        exception.Message!.ShouldContain("The current user is not authorized to access this resource.");
    }
}
