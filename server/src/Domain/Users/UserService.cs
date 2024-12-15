namespace Domain.Users;

public sealed class UserService(IUserRepository userRepository, IAuthService authService)
{
    public async Task<ErrorOr<TokensPair>> Login(string email, string password)
    {
        var user = await userRepository.FindUserByEmail(email);

        if (user.Errors.Contains(User.Errors.NotFound))
            return User.Errors.IncorrectEmailOrPassword;

        if (user.Value.VerifyPasswordHash(password) is false)
            return User.Errors.IncorrectEmailOrPassword;

        var userSession = UserSession.Create(user.Value.UserId);

        if (await AreThereTooManySessionsPerUser(userSession.UserId))
            await userRepository.DeleteAllUserSessionsForUser(userSession.UserId);

        await userRepository.AddUserSession(userSession);
        await userRepository.SaveChanges();

        var accessToken = authService.CreateAccessToken(user.Value);

        return (accessToken, userSession.RefreshToken);
    }

    public async Task<ErrorOr<TokensPair>> RefreshTokens(AccessToken expiredAccessToken, RefreshToken refreshToken)
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

        return (newAccessToken, userSession.Value.RefreshToken);
    }

    private async Task<bool> AreThereTooManySessionsPerUser(Guid userId) =>
        await userRepository.GetNumberOfUserSessionsForUser(userId) >= UserSession.MaxSessionsPerUser;
}