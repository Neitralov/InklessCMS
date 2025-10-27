namespace WebAPI.IntegrationTests.CollectionsControllerEndpoints;

[Collection("Tests")]
public sealed class GetCollectionsTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
    private readonly CustomWebApplicationFactory _factory = factory;

    [Fact]
    public async Task EmptyListWillBeReturnedIfNoCollectionsExist()
    {
        // Arrange
        var gqlClient = _factory.CreateClient().ToGqlClient();

        // Act
        var gqlResponse = await gqlClient.GetCollections();

        // Assert
        gqlResponse.ShouldBeEmpty();
    }

    [Fact]
    public async Task CollectionsWillBeReturnedIfCollectionsExist()
    {
        // Arrange
        var gqlAdminClient = _factory.AuthorizeAs(UserTypes.Admin).CreateClient().ToGqlClient();
        const int numberOfCollections = 2;

        for (var index = 1; index <= numberOfCollections; index++)
            await gqlAdminClient.CreateCollection(Inputs.Collection.CollectionInput with
            {
                CollectionId = $"collection-{index}"
            });

        var gqlClient = _factory.CreateClient().ToGqlClient();

        // Act
        var gqlResponse = await gqlClient.GetCollections();

        // Assert
        gqlResponse.Count.ShouldBe(numberOfCollections);
    }
}
