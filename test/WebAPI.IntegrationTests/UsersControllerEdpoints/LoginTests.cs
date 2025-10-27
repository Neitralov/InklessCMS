namespace WebAPI.IntegrationTests.UsersControllerEdpoints;

[Collection("Tests")]
public sealed class LoginTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
    private readonly CustomWebApplicationFactory _factory = factory;

    [Fact]
    public async Task ItIsPossibleToLoginInAdminAccount()
    {
        // Arrange
        var gqlClient = _factory.CreateClient().ToGqlClient();

        // Act
        var gqlResponse = await gqlClient.Login(Inputs.User.LoginInput);

        // Assert
        gqlResponse.RefreshToken.ShouldNotBeEmpty();
        gqlResponse.AccessToken.ShouldNotBeEmpty();
    }

    [Fact]
    public async Task ItIsImpossibleToLoginInAdminAccountWithInvalidData()
    {
        // Arrange
        var gqlClient = _factory.CreateClient().ToGqlClient();

        // Act
        var exception = await Should.ThrowAsync<GraphQLException>(async () =>
        {
            await gqlClient.Login(Inputs.User.LoginInput with { Password = "Invalid" });
        });

        // Assert
        exception.Message.ShouldBe(User.Errors.IncorrectEmailOrPassword.Code);
    }
}
