namespace Domain.Tests.Authorization;

public sealed class UserSessionTests
{
    [Fact]
    public void Valid_user_session_can_be_created()
    {
        // Arrange
        var user = User.Create(
            email: "example@gmail.com",
            password: "1234");

        var sut = UserSession.Create(user.Value.UserId);

        // Act
        var result = Math.Round((sut.ExpirationDate - DateTime.UtcNow).TotalDays);

        // Assert
        result.Should().Be(UserSession.ExpiresInDays);
    }

    [Fact]
    public void User_session_can_be_updated()
    {
        // Arrange
        var user = User.Create(
            email: "example@gmail.com",
            password: "1234");

        var sut = UserSession.Create(user.Value.UserId);

        // Act
        var oldRefreshTokenResult = sut.RefreshToken;
        sut.Update();
        var newRefreshTokenResult = sut.RefreshToken;

        // Assert
        oldRefreshTokenResult.Should().NotBeEquivalentTo(newRefreshTokenResult);
    }
}
