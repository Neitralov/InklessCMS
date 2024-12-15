namespace Domain.Users;

public sealed class UserService(IUserRepository userRepository, IAuthService authService)
{
    public async Task<ErrorOr<TokensPair>> Login(string email, string password)
    {
        var user = await userRepository.FindUserByEmail(email);

        if (user.Errors.Contains(Errors.User.NotFound))
            return Errors.Login.IncorrectEmailOrPassword;

        if (user.Value.VerifyPasswordHash(password) is false)
            return Errors.Login.IncorrectEmailOrPassword;

        var userSession = UserSession.Create(user.Value.UserId);

        if (await AreThereTooManySessionsPerUser(userSession.UserId))
            await userRepository.DeleteAllUserSessionsForUser(userSession.UserId);

        await userRepository.AddUserSession(userSession);
        await userRepository.SaveChanges();

        var accessToken = authService.CreateAccessToken(user.Value);
        var refreshToken = userSession.Token;

        return (accessToken, refreshToken);
    }

    public async Task<ErrorOr<TokensPair>> RefreshTokens(string expiredAccessToken, string refreshToken)
    {
        var userEmail = authService.GetEmailFromJwt(expiredAccessToken);

        if (userEmail.IsError)
            return userEmail.FirstError;

        var user = await userRepository.FindUserByEmail(userEmail.Value);

        if (user.IsError)
            return user.Errors;

        var userSession = await userRepository.GetUserSession(user.Value.UserId, refreshToken);

        if (userSession.IsError)
            return userSession.Errors;

        await userSession.Value.Update();
        await userRepository.SaveChanges();

        var newAccessToken = authService.CreateAccessToken(user.Value);
        var newRefreshToken = userSession.Value.Token;

        return (newAccessToken, newRefreshToken);
    }

    private async Task<bool> AreThereTooManySessionsPerUser(Guid userId) =>
        await userRepository.GetNumberOfUserSessionsForUser(userId) >= UserSession.MaxSessionsPerUser;
}