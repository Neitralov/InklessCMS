namespace WebAPI.IntegrationTests.CollectionsControllerEndpoints;

[Collection("Tests")]
public sealed class UpdateCollectionTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
    private readonly CustomWebApplicationFactory _factory = factory;

    [Fact]
    public async Task CollectionCanBeUpdated()
    {
        // Arrange
        var gqlClient = _factory.AuthorizeAs(UserTypes.Admin).CreateClient().ToGqlClient();
        const string defaultTitle = "Default title";
        const string updatedTitle = "Updated title";

        const string collectionId = "collection-id";
        
        // Act
        var createCollectionResponse = await gqlClient.CreateCollection(Requests.Collection.CollectionInput with
        {
            CollectionId = collectionId,
            Title = defaultTitle
        });

        var updateCollectionResponse = await gqlClient.UpdateCollection(Requests.Collection.CollectionInput with
        {
            CollectionId = collectionId,
            Title = updatedTitle
        });

        // Assert
        createCollectionResponse.Title.ShouldBe(defaultTitle);
        updateCollectionResponse.Title.ShouldBe(updatedTitle);
    }

    [Fact]
    public async Task CollectionCannotBeUpdatedIfItDoesNotExist()
    {
        // Arrange
        var gqlClient = _factory.AuthorizeAs(UserTypes.Admin).CreateClient().ToGqlClient();
        const string collectionId = "collection-id";

        // Act
        var exception = await Should.ThrowAsync<GraphQLHttpRequestException>(async () =>
        {
            await gqlClient.UpdateCollection(Requests.Collection.CollectionInput with { CollectionId = collectionId });
        });

        // Assert
        exception.Content!.ShouldContain(Collection.Errors.NotFound.Code);
    }

    [Fact]
    public async Task CollectionCannotBeUpdatedWithInvalidData()
    {
        // Arrange
        var gqlClient = _factory.AuthorizeAs(UserTypes.Admin).CreateClient().ToGqlClient();
        const string defaultTitle = "Default title";
        const string tooShortTitle = "Aa";

        const string collectionId = "collection-id";
        await gqlClient.CreateCollection(Requests.Collection.CollectionInput with
        {
            CollectionId = collectionId,
            Title = defaultTitle
        });

        // Act
        var exception = await Should.ThrowAsync<GraphQLHttpRequestException>(async () =>
        {
            await gqlClient.UpdateCollection(Requests.Collection.CollectionInput with
            {
                CollectionId = collectionId,
                Title = tooShortTitle
            });
        });

        // Assert
        exception.Content!.ShouldContain(Collection.Errors.InvalidTitleLength.Code);
    }

    [Fact]
    public async Task OnlyAuthorizedUserCanUpdateCollection()
    {
        // Arrange
        var gqlClient = _factory.CreateClient().ToGqlClient();

        // Act
        var exception = await Should.ThrowAsync<GraphQLHttpRequestException>(async () =>
        {
            await gqlClient.UpdateCollection(Requests.Collection.CollectionInput);
        });

        // Assert
        exception.Content!.ShouldContain("AUTH_NOT_AUTHORIZED");
    }

    [Fact]
    public async Task OnlyUserWithCanManageArticlesClaimCanUpdateCollection()
    {
        // Arrange
        var gqlClient = _factory.AuthorizeAs(UserTypes.User).CreateClient().ToGqlClient();

        // Act
        var exception = await Should.ThrowAsync<GraphQLHttpRequestException>(async () =>
        {
            await gqlClient.UpdateCollection(Requests.Collection.CollectionInput);
        });

        // Assert
        exception.Content!.ShouldContain("AUTH_NOT_AUTHORIZED");
    }
}
