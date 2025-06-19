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
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var loginResponse = await response.Content.ReadFromJsonAsync<LoginUserResponse>();
        loginResponse?.RefreshToken.ShouldNotBeEmpty();
        loginResponse?.AccessToken.ShouldNotBeEmpty();
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
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}
