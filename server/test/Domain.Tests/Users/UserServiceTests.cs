namespace Domain.Tests.Users;

public sealed class UserServiceTests
{
    private Mock<IUserRepository> _userRepositoryMock = new();
    private Mock<IAuthService> _authServiceMock = new();

    private const string Email = "example@gmail.com";
    private const string Password = "1234";
    private const string SecretKey = "My favorite really secret key. 512 bit at least. (64 characters).";

    private const string ExpiredAccessToken = "Expired access token";
    private const string RefreshToken = "Refresh token";
    private const string ExpiredRefreshToken = "Expired refresh token";

    private User _user = User.Create(
        email: Email,
        password: Password).Value;

    private UserSession _userSession;

    public UserServiceTests() => _userSession = UserSession.Create(_user.UserId);

    [Fact]
    public async Task User_can_login_with_valid_data()
    {
        // Arrange
        _userRepositoryMock
            .Setup(repository => repository.FindUserByEmail(It.IsAny<string>()).Result)
            .Returns(_user);
        _userRepositoryMock
            .Setup(repository => repository.GetNumberOfUserSessionsForUser(It.IsAny<Guid>()).Result)
            .Returns(0);

        var jwtOptions = Microsoft.Extensions.Options.Options.Create(new JwtOptions { SecretKey = SecretKey });
        var authService = new AuthService(jwtOptions);
        var sut = new UserService(_userRepositoryMock.Object, authService);

        // Act
        var result = await sut.Login(Email, Password);

        // Assert
        result.IsError.Should().BeFalse();
        _userRepositoryMock.Verify(repository => repository.AddUserSession(It.IsAny<UserSession>()), Times.Once);
        _userRepositoryMock.Verify(repository => repository.SaveChanges(), Times.Once);
        _userRepositoryMock.Verify(repository =>
            repository.DeleteAllUserSessionsForUser(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task User_cant_login_if_he_has_no_account()
    {
        // Arrange
        _userRepositoryMock
            .Setup(repository => repository.FindUserByEmail(It.IsAny<string>()).Result)
            .Returns(Domain.Users.Errors.User.NotFound);
        _userRepositoryMock
            .Setup(repository => repository.GetNumberOfUserSessionsForUser(It.IsAny<Guid>()).Result)
            .Returns(0);

        var jwtOptions = Microsoft.Extensions.Options.Options.Create(new JwtOptions { SecretKey = SecretKey });
        var authService = new AuthService(jwtOptions);
        var sut = new UserService(_userRepositoryMock.Object, authService);

        // Act
        var result = await sut.Login(Email, Password);

        // Assert
        result.FirstError.Should().Be(Domain.Users.Errors.Login.IncorrectEmailOrPassword);
        _userRepositoryMock.Verify(repository => repository.AddUserSession(It.IsAny<UserSession>()), Times.Never);
        _userRepositoryMock.Verify(repository => repository.SaveChanges(), Times.Never);
        _userRepositoryMock.Verify(repository =>
            repository.DeleteAllUserSessionsForUser(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task User_cant_login_with_incorrect_email()
    {
        // Arrange
        _userRepositoryMock
            .Setup(repository => repository.FindUserByEmail(It.IsAny<string>()).Result)
            .Returns(Domain.Users.Errors.User.NotFound);
        _userRepositoryMock
            .Setup(repository => repository.GetNumberOfUserSessionsForUser(It.IsAny<Guid>()).Result)
            .Returns(0);

        var jwtOptions = Microsoft.Extensions.Options.Options.Create(new JwtOptions { SecretKey = SecretKey });
        var authService = new AuthService(jwtOptions);
        var sut = new UserService(_userRepositoryMock.Object, authService);

        // Act
        var result = await sut.Login("incorrect@mail.ru", Password);

        // Assert
        result.FirstError.Should().Be(Domain.Users.Errors.Login.IncorrectEmailOrPassword);
        _userRepositoryMock.Verify(repository => repository.AddUserSession(It.IsAny<UserSession>()), Times.Never);
        _userRepositoryMock.Verify(repository => repository.SaveChanges(), Times.Never);
        _userRepositoryMock.Verify(repository =>
            repository.DeleteAllUserSessionsForUser(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task User_cant_login_with_incorrect_password()
    {
        // Arrange
        _userRepositoryMock
            .Setup(repository => repository.FindUserByEmail(Email).Result)
            .Returns(_user);
        _userRepositoryMock
            .Setup(repository => repository.GetNumberOfUserSessionsForUser(It.IsAny<Guid>()).Result)
            .Returns(0);

        var jwtOptions = Microsoft.Extensions.Options.Options.Create(new JwtOptions { SecretKey = SecretKey });
        var authService = new AuthService(jwtOptions);
        var sut = new UserService(_userRepositoryMock.Object, authService);

        // Act
        var result = await sut.Login(Email, "incorrect password");

        // Assert
        result.FirstError.Should().Be(Domain.Users.Errors.Login.IncorrectEmailOrPassword);
        _userRepositoryMock.Verify(repository => repository.AddUserSession(It.IsAny<UserSession>()), Times.Never);
        _userRepositoryMock.Verify(repository => repository.SaveChanges(), Times.Never);
        _userRepositoryMock.Verify(repository =>
            repository.DeleteAllUserSessionsForUser(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task UserSessions_will_be_deleted_after_user_login_if_there_is_too_many_user_sessions()
    {
        // Arrange
        _userRepositoryMock
            .Setup(repository => repository.FindUserByEmail(It.IsAny<string>()).Result)
            .Returns(_user);
        _userRepositoryMock
            .Setup(repository => repository.GetNumberOfUserSessionsForUser(It.IsAny<Guid>()).Result)
            .Returns(UserSession.MaxSessionsPerUser);

        var jwtOptions = Microsoft.Extensions.Options.Options.Create(new JwtOptions { SecretKey = SecretKey });
        var authService = new AuthService(jwtOptions);
        var sut = new UserService(_userRepositoryMock.Object, authService);

        // Act
        var result = await sut.Login(Email, Password);

        // Assert
        result.IsError.Should().BeFalse();
        _userRepositoryMock.Verify(repository => repository.AddUserSession(It.IsAny<UserSession>()), Times.Once);
        _userRepositoryMock.Verify(repository => repository.SaveChanges(), Times.Once);
        _userRepositoryMock.Verify(repository => repository.DeleteAllUserSessionsForUser(It.IsAny<Guid>()), Times.Once);
    }

    [Fact]
    public async Task User_can_refresh_his_tokens()
    {
        // Arrange
        _userRepositoryMock
            .Setup(repository => repository.FindUserByEmail(It.IsAny<string>()).Result)
            .Returns(_user);
        _userRepositoryMock
            .Setup(repository => repository.GetUserSession(It.IsAny<Guid>(), It.IsAny<string>()).Result)
            .Returns(_userSession);

        _authServiceMock
            .Setup(service => service.GetEmailFromJwt(ExpiredAccessToken))
            .Returns(Email);

        var sut = new UserService(_userRepositoryMock.Object, _authServiceMock.Object);

        // Act
        var result = await sut.RefreshTokens(ExpiredAccessToken, RefreshToken);

        // Assert
        result.IsError.Should().BeFalse();
        _userRepositoryMock.Verify(repository => repository.SaveChanges(), Times.Once);
        _authServiceMock.Verify(service => service.CreateAccessToken(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task User_cant_refresh_his_tokens_when_access_token_is_invalid()
    {
        // Arrange
        _userRepositoryMock
            .Setup(repository => repository.FindUserByEmail(It.IsAny<string>()).Result)
            .Returns(_user);
        _userRepositoryMock
            .Setup(repository => repository.GetUserSession(It.IsAny<Guid>(), It.IsAny<string>()).Result)
            .Returns(_userSession);

        _authServiceMock
            .Setup(service => service.GetEmailFromJwt(It.IsAny<string>()))
            .Returns(Domain.Authorization.Errors.AccessToken.InvalidToken);

        var sut = new UserService(_userRepositoryMock.Object, _authServiceMock.Object);

        // Act
        var result = await sut.RefreshTokens("Invalid access token", RefreshToken);

        // Assert
        result.FirstError.Should().Be(Domain.Authorization.Errors.AccessToken.InvalidToken);
        _userRepositoryMock.Verify(repository => repository.SaveChanges(), Times.Never);
        _authServiceMock.Verify(service => service.CreateAccessToken(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task User_cant_refresh_his_tokens_if_account_is_deleted()
    {
        // Arrange
        _userRepositoryMock
            .Setup(repository => repository.FindUserByEmail(It.IsAny<string>()).Result)
            .Returns(Domain.Users.Errors.User.NotFound);
        _userRepositoryMock
            .Setup(repository => repository.GetUserSession(It.IsAny<Guid>(), It.IsAny<string>()).Result)
            .Returns(_userSession);

        _authServiceMock
            .Setup(service => service.GetEmailFromJwt(ExpiredAccessToken))
            .Returns(Email);

        var sut = new UserService(_userRepositoryMock.Object, _authServiceMock.Object);

        // Act
        var result = await sut.RefreshTokens(ExpiredAccessToken, RefreshToken);

        // Assert
        result.FirstError.Should().Be(Domain.Users.Errors.User.NotFound);
        _userRepositoryMock.Verify(repository => repository.SaveChanges(), Times.Never);
        _authServiceMock.Verify(service => service.CreateAccessToken(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task User_cant_refresh_his_tokens_if_refresh_token_is_expired()
    {
        // Arrange
        _userRepositoryMock
            .Setup(repository => repository.FindUserByEmail(It.IsAny<string>()).Result)
            .Returns(_user);
        _userRepositoryMock
            .Setup(repository => repository.GetUserSession(It.IsAny<Guid>(), It.IsAny<string>()).Result)
            .Returns(Domain.Authorization.Errors.UserSession.InvalidToken);

        _authServiceMock
            .Setup(service => service.GetEmailFromJwt(ExpiredAccessToken))
            .Returns(Email);

        var sut = new UserService(_userRepositoryMock.Object, _authServiceMock.Object);

        // Act
        var result = await sut.RefreshTokens(ExpiredAccessToken, ExpiredRefreshToken);

        // Assert
        result.FirstError.Should().Be(Domain.Authorization.Errors.UserSession.InvalidToken);
        _userRepositoryMock.Verify(repository => repository.SaveChanges(), Times.Never);
        _authServiceMock.Verify(service => service.CreateAccessToken(It.IsAny<User>()), Times.Never);
    }
}