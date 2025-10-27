using Domain.Authorization;

namespace WebAPI.IntegrationTests.UsersControllerEdpoints;

[Collection("Tests")]
public sealed class RefreshTokensTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
    private readonly CustomWebApplicationFactory _factory = factory;

    [Fact]
    public async Task ItIsPossibleToRefreshActualTokens()
    {
        // Arrange
        var gqlClient = _factory.CreateClient().ToGqlClient();

        var creditnails = await gqlClient.Login(Requests.User.LoginInput);

        // Act
        var gqlResponse = await gqlClient.RefreshTokens(Requests.User.RefreshTokenInput with
        {
            ExpiredAccessToken = creditnails.AccessToken,
            RefreshToken = creditnails.RefreshToken
        });

        // Assert
        gqlResponse.RefreshToken.ShouldNotBeEmpty();
        gqlResponse.AccessToken.ShouldNotBeEmpty();
    }

    [Fact]
    public async Task ItIsImpossibleToRefreshInvalidAccessTokens()
    {
        // Arrange
        var gqlClient = _factory.CreateClient().ToGqlClient();

        var creditnails = await gqlClient.Login(Requests.User.LoginInput);

        // Act
        var exception = await Should.ThrowAsync<GraphQLException>(async () =>
        {
            await gqlClient.RefreshTokens(Requests.User.RefreshTokenInput with
            {
                ExpiredAccessToken = "invalid-access-token",
                RefreshToken = creditnails.RefreshToken
            });
        });

        // Assert
        exception.Message.ShouldBe(AccessToken.Errors.InvalidToken.Code);
    }

    [Fact]
    public Task ItIsImpossibleToRefreshTokensForUserThatDoesNotExist()
    {
        // Сценарий возможен лишь теоретически.

        return Task.CompletedTask;
    }
}
