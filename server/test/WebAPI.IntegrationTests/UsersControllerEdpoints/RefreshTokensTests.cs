namespace WebAPI.IntegrationTests.UsersControllerEdpoints;

[Collection("Tests")]
public sealed class RefreshTokensTests(CustomWebApplicationFactory factory) : IAsyncLifetime
{
    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync() => await factory.ResetDatabaseAsync();
    
    [Fact]
    public async Task ItIsPossibleToRefreshActualTokens()
    {
        // Arrange
        var client = factory.CreateClient();
        
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
        var client = factory.CreateClient();
        
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
        // Сценарий возможен лишь теоретически. Чтобы это провернуть, нужно рефрешнуть токены пользователя,
        //   который удалил свою учетку, но сейчас такой возможности нет.
        
        return Task.CompletedTask;
    }
}