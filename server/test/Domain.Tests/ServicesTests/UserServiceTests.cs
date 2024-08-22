using Domain.Options;

namespace Domain.Tests.ServicesTests;

public class UserServiceTests
{
    [Fact]
    public async Task User_can_login_with_valid_data()
    {
        const string email = "example@gmail.com";
        const string password = "1234";
        const string secretKey = "My favorite really secret key. 512 bit at least. (64 characters).";
        var user = User.Create(
            email: email,
            password: password);

        var userRepositoryMock = new Mock<IUserRepository>();
        userRepositoryMock.Setup(repository => repository.FindUserByEmail(It.IsAny<string>()).Result).Returns(user.Value);
        userRepositoryMock.Setup(repository => repository.GetNumberOfUserSessionsForUser(It.IsAny<Guid>()).Result).Returns(0);
        var jwtOptionsMock = Microsoft.Extensions.Options.Options.Create(new JwtOptions { SecretKey = secretKey });
        var authService = new AuthService(jwtOptionsMock);
        var sut = new UserService(userRepositoryMock.Object, authService);

        var result = await sut.Login(email, password);

        result.IsError.Should().BeFalse();
        userRepositoryMock.Verify(repository => repository.AddUserSession(It.IsAny<UserSession>()), Times.Once);
        userRepositoryMock.Verify(repository => repository.SaveChanges(), Times.Once);
        userRepositoryMock.Verify(repository => repository.DeleteAllUserSessionsForUser(It.IsAny<Guid>()), Times.Never);
    }
    
    [Fact]
    public async Task User_cant_login_if_he_has_no_account()
    {
        const string email = "example@gmail.com";
        const string password = "1234";
        const string secretKey = "My favorite really secret key. 512 bit at least. (64 characters).";

        var userRepositoryMock = new Mock<IUserRepository>();
        userRepositoryMock.Setup(repository => repository.FindUserByEmail(It.IsAny<string>()).Result).Returns(value: null);
        userRepositoryMock.Setup(repository => repository.GetNumberOfUserSessionsForUser(It.IsAny<Guid>()).Result).Returns(0);
        var jwtOptionsMock = Microsoft.Extensions.Options.Options.Create(new JwtOptions { SecretKey = secretKey });
        var authService = new AuthService(jwtOptionsMock);
        var sut = new UserService(userRepositoryMock.Object, authService);

        var result = await sut.Login(email, password);

        result.FirstError.Should().Be(Errors.Login.IncorrectEmailOrPassword);
        userRepositoryMock.Verify(repository => repository.AddUserSession(It.IsAny<UserSession>()), Times.Never);
        userRepositoryMock.Verify(repository => repository.SaveChanges(), Times.Never);
        userRepositoryMock.Verify(repository => repository.DeleteAllUserSessionsForUser(It.IsAny<Guid>()), Times.Never);
    }
    
    [Fact]
    public async Task User_cant_login_with_incorrect_email()
    {
        const string email = "example@gmail.com";
        const string password = "1234";
        const string secretKey = "My favorite really secret key. 512 bit at least. (64 characters).";
        var user = User.Create(
            email: email,
            password: password);

        var userRepositoryMock = new Mock<IUserRepository>();
        userRepositoryMock.Setup(repository => repository.FindUserByEmail(email).Result).Returns(user.Value);
        userRepositoryMock.Setup(repository => repository.GetNumberOfUserSessionsForUser(It.IsAny<Guid>()).Result).Returns(0);
        var jwtOptionsMock = Microsoft.Extensions.Options.Options.Create(new JwtOptions { SecretKey = secretKey });
        var authService = new AuthService(jwtOptionsMock);
        var sut = new UserService(userRepositoryMock.Object, authService);

        var result = await sut.Login("incorrect@mail.ru", password);

        result.FirstError.Should().Be(Errors.Login.IncorrectEmailOrPassword);
        userRepositoryMock.Verify(repository => repository.AddUserSession(It.IsAny<UserSession>()), Times.Never);
        userRepositoryMock.Verify(repository => repository.SaveChanges(), Times.Never);
        userRepositoryMock.Verify(repository => repository.DeleteAllUserSessionsForUser(It.IsAny<Guid>()), Times.Never);
    }
    
    [Fact]
    public async Task User_cant_login_with_incorrect_password()
    {
        const string email = "example@gmail.com";
        const string password = "1234";
        const string secretKey = "My favorite really secret key. 512 bit at least. (64 characters).";
        var user = User.Create(
            email: email,
            password: password);

        var userRepositoryMock = new Mock<IUserRepository>();
        userRepositoryMock.Setup(repository => repository.FindUserByEmail(email).Result).Returns(user.Value);
        userRepositoryMock.Setup(repository => repository.GetNumberOfUserSessionsForUser(It.IsAny<Guid>()).Result).Returns(0);
        var jwtOptionsMock = Microsoft.Extensions.Options.Options.Create(new JwtOptions { SecretKey = secretKey });
        var authService = new AuthService(jwtOptionsMock);
        var sut = new UserService(userRepositoryMock.Object, authService);

        var result = await sut.Login(email, "qwerty");

        result.FirstError.Should().Be(Errors.Login.IncorrectEmailOrPassword);
        userRepositoryMock.Verify(repository => repository.AddUserSession(It.IsAny<UserSession>()), Times.Never);
        userRepositoryMock.Verify(repository => repository.SaveChanges(), Times.Never);
        userRepositoryMock.Verify(repository => repository.DeleteAllUserSessionsForUser(It.IsAny<Guid>()), Times.Never);
    }
    
    [Fact]
    public async Task UserSessions_will_be_deleted_after_user_login_if_there_is_too_many_user_sessions()
    {
        const string email = "example@gmail.com";
        const string password = "1234";
        const string secretKey = "My favorite really secret key. 512 bit at least. (64 characters).";
        var user = User.Create(
            email: email,
            password: password);

        var userRepositoryMock = new Mock<IUserRepository>();
        userRepositoryMock.Setup(repository => repository.FindUserByEmail(It.IsAny<string>()).Result).Returns(user.Value);
        userRepositoryMock.Setup(repository => repository.GetNumberOfUserSessionsForUser(It.IsAny<Guid>()).Result).Returns(UserSession.MaxSessionsPerUser);
        var jwtOptionsMock = Microsoft.Extensions.Options.Options.Create(new JwtOptions { SecretKey = secretKey });
        var authService = new AuthService(jwtOptionsMock);
        var sut = new UserService(userRepositoryMock.Object, authService);

        var result = await sut.Login(email, password);

        result.IsError.Should().BeFalse();
        userRepositoryMock.Verify(repository => repository.AddUserSession(It.IsAny<UserSession>()), Times.Once);
        userRepositoryMock.Verify(repository => repository.SaveChanges(), Times.Once);
        userRepositoryMock.Verify(repository => repository.DeleteAllUserSessionsForUser(It.IsAny<Guid>()), Times.Once);
    }
    
    [Fact]
    public async Task User_can_refresh_his_tokens()
    {
        const string email = "example@gmail.com";
        const string password = "1234";
        var user = User.Create(
            email: email,
            password: password);
        var userSession = UserSession.Create(user.Value.UserId);

        var userRepositoryMock = new Mock<IUserRepository>();
        userRepositoryMock.Setup(repository => repository.FindUserByEmail(It.IsAny<string>()).Result).Returns(user.Value);
        userRepositoryMock.Setup(repository => repository.GetUserSession(It.IsAny<Guid>(), It.IsAny<string>()).Result).Returns(userSession);
        var authServiceMock = new Mock<IAuthService>();
        authServiceMock.Setup(service => service.GetEmailFromJwt("Valid token")).Returns(email);
        var sut = new UserService(userRepositoryMock.Object, authServiceMock.Object);
        
        var result = await sut.RefreshTokens("Valid token", "Valid token");

        result.IsError.Should().BeFalse();
        userRepositoryMock.Verify(repository => repository.SaveChanges(), Times.Once);
        authServiceMock.Verify(service => service.CreateAccessToken(It.IsAny<User>()), Times.Once);
    }
    
    [Fact]
    public async Task User_cant_refresh_his_tokens_when_access_token_is_invalid()
    {
        const string email = "example@gmail.com";
        const string password = "1234";
        var user = User.Create(
            email: email,
            password: password);
        var userSession = UserSession.Create(user.Value.UserId);

        var userRepositoryMock = new Mock<IUserRepository>();
        userRepositoryMock.Setup(repository => repository.FindUserByEmail(It.IsAny<string>()).Result).Returns(user.Value);
        userRepositoryMock.Setup(repository => repository.GetUserSession(It.IsAny<Guid>(), It.IsAny<string>()).Result).Returns(userSession);
        var authServiceMock = new Mock<IAuthService>();
        authServiceMock.Setup(service => service.GetEmailFromJwt(It.IsAny<string>())).Returns(Errors.AccessToken.InvalidToken);
        var sut = new UserService(userRepositoryMock.Object, authServiceMock.Object);
        
        var result = await sut.RefreshTokens("Invalid token", "Valid token");

        result.FirstError.Should().Be(Errors.AccessToken.InvalidToken);
        userRepositoryMock.Verify(repository => repository.SaveChanges(), Times.Never);
        authServiceMock.Verify(service => service.CreateAccessToken(It.IsAny<User>()), Times.Never);
    }
    
    [Fact]
    public async Task User_cant_refresh_his_tokens_if_account_is_deleted()
    {
        const string email = "example@gmail.com";
        const string password = "1234";
        var user = User.Create(
            email: email,
            password: password);
        var userSession = UserSession.Create(user.Value.UserId);

        var userRepositoryMock = new Mock<IUserRepository>();
        userRepositoryMock.Setup(repository => repository.FindUserByEmail(It.IsAny<string>()).Result).Returns(value: null);
        userRepositoryMock.Setup(repository => repository.GetUserSession(It.IsAny<Guid>(), It.IsAny<string>()).Result).Returns(userSession);
        var authServiceMock = new Mock<IAuthService>();
        authServiceMock.Setup(service => service.GetEmailFromJwt("Valid token")).Returns(email);
        var sut = new UserService(userRepositoryMock.Object, authServiceMock.Object);
        
        var result = await sut.RefreshTokens("Valid token", "Valid token");

        result.FirstError.Should().Be(Errors.User.NotFound);
        userRepositoryMock.Verify(repository => repository.SaveChanges(), Times.Never);
        authServiceMock.Verify(service => service.CreateAccessToken(It.IsAny<User>()), Times.Never);
    }
    
    [Fact]
    public async Task User_cant_refresh_his_tokens_if_refresh_token_is_expired()
    {
        const string email = "example@gmail.com";
        const string password = "1234";
        var user = User.Create(
            email: email,
            password: password);
        
        var userRepositoryMock = new Mock<IUserRepository>();
        userRepositoryMock.Setup(repository => repository.FindUserByEmail(It.IsAny<string>()).Result).Returns(user.Value);
        userRepositoryMock.Setup(repository => repository.GetUserSession(It.IsAny<Guid>(), It.IsAny<string>()).Result).Returns(value: null);
        var authServiceMock = new Mock<IAuthService>();
        authServiceMock.Setup(service => service.GetEmailFromJwt("Valid token")).Returns(email);
        var sut = new UserService(userRepositoryMock.Object, authServiceMock.Object);
        
        var result = await sut.RefreshTokens("Valid token", "Expired token");

        result.FirstError.Should().Be(Errors.UserSession.InvalidToken);
        userRepositoryMock.Verify(repository => repository.SaveChanges(), Times.Never);
        authServiceMock.Verify(service => service.CreateAccessToken(It.IsAny<User>()), Times.Never);
    }
}