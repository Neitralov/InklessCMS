namespace WebAPI.IntegrationTests.UsersControllerEdpoints;

[Collection("Tests")]
public sealed class LoginTests(CustomWebApplicationFactory factory) : IAsyncLifetime
{
    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync() => await factory.ResetDatabaseAsync();

    [Fact]
    public async Task ItIsPossibleToLoginInAdminAccount()
    {
        // Arrange
        var client = factory.CreateClient();
        
        // Act
        var response = await client.PostAsJsonAsync(
            requestUri: "/api/users/login",
            value: DataGenerator.User.GetLoginUserRequest());
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var loginResponse = await response.Content.ReadFromJsonAsync<LoginUserResponse>();
        loginResponse?.RefreshToken.Should().NotBeEmpty();
        loginResponse?.AccessToken.Should().NotBeEmpty();
    }
    
    [Fact]
    public async Task ItIsImpossibleToLoginInAdminAccountWithInvalidData()
    {
        // Arrange
        var client = factory.CreateClient();
        
        // Act
        var response = await client.PostAsJsonAsync(
            requestUri: "/api/users/login",
            value: DataGenerator.User.GetLoginUserRequest() with { Password = "invalid" });
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}