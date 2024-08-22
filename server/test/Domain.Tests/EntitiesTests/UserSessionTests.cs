namespace Domain.Tests.EntitiesTests;

public class UserSessionTests
{
    [Fact]
    public void Valid_user_session_can_be_created()
    {
        var user = User.Create(
            email: "example@gmail.com",
            password: "1234");

        var sut = UserSession.Create(user.Value.UserId);

        var result = Math.Round((sut.ExpirationDate - DateTime.UtcNow).TotalDays);

        result.Should().Be(UserSession.ExpiresInDays);
    }
    
    [Fact]
    public void User_session_can_be_updated()
    {
        var user = User.Create(
            email: "example@gmail.com",
            password: "1234");

        var sut = UserSession.Create(user.Value.UserId);

        var oldTokenResult = sut.Token;
        sut.Update();
        var newTokenResult = sut.Token;

        oldTokenResult.Should().NotBeEquivalentTo(newTokenResult);
    }
}