namespace WebAPI.IntegrationTests.UsersControllerEdpoints;

[Collection("Tests")]
public sealed class RefreshTokensTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
    private readonly CustomWebApplicationFactory _factory = factory;

    [Fact]
    public async Task ItIsPossibleToRefreshActualTokens()
    {
        // Arrange
        var client = _factory.CreateClient();

        var getLoginUserResponse = await client.PostAsJsonAsync(
            requestUri: "/api/users/login",
            value: DataGenerator.User.GetLoginUserRequest());

        var creditnails = (await getLoginUserResponse.Content.ReadFromJsonAsync<LoginUserResponse>())!;

        // Act
        var response = await client.PostAsJsonAsync(
            requestUri: "/api/users/refresh-tokens",
            value: DataGenerator.User.GetRefreshTokenRequest() with
            {
                RefreshToken = creditnails.RefreshToken,
                ExpiredAccessToken = creditnails.AccessToken
            });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var loginResponse = await response.Content.ReadFromJsonAsync<LoginUserResponse>();
        loginResponse?.RefreshToken.Should().NotBeEmpty();
        loginResponse?.AccessToken.Should().NotBeEmpty();
    }

    [Fact]
    public async Task ItIsImpossibleToRefreshInvalidAccessTokens()
    {
        // Arrange
        var client = _factory.CreateClient();

        var getLoginUserResponse = await client.PostAsJsonAsync(
            requestUri: "/api/users/login",
            value: DataGenerator.User.GetLoginUserRequest());

        var creditnails = (await getLoginUserResponse.Content.ReadFromJsonAsync<LoginUserResponse>())!;

        // Act
        var response = await client.PostAsJsonAsync(
            requestUri: "/api/users/refresh-tokens",
            value: DataGenerator.User.GetRefreshTokenRequest() with
            {
                RefreshToken = creditnails.RefreshToken,
                ExpiredAccessToken = "invalid-access-token"
            });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public Task ItIsImpossibleToRefreshTokensForUserThatDoesNotExist()
    {
        // Сценарий возможен лишь теоретически.

        return Task.CompletedTask;
    }
}
