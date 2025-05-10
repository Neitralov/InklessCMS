namespace WebAPI.IntegrationTests.UsersControllerEdpoints;

[Collection("Tests")]
public sealed class LoginTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
    private readonly CustomWebApplicationFactory _factory = factory;

    [Fact]
    public async Task ItIsPossibleToLoginInAdminAccount()
    {
        // Arrange
        var client = _factory.CreateClient();

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
        var client = _factory.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync(
            requestUri: "/api/users/login",
            value: DataGenerator.User.GetLoginUserRequest() with { Password = "invalid" });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
