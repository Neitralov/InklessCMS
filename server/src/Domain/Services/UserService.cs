namespace Domain.Services;

public class UserService(IUserRepository userRepository, IAuthService authService)
{
    public async Task<ErrorOr<TokensPair>> Login(string email, string password)
    {
        var user = await userRepository.FindUserByEmail(email);

        if (user is null)
            return Errors.Login.IncorrectEmailOrPassword;

        if (user.VerifyPasswordHash(password) is false)
            return Errors.Login.IncorrectEmailOrPassword;

        var userSession = UserSession.Create(user.UserId);

        if (await AreThereTooManySessionsPerUser(userSession.UserId))
            await userRepository.DeleteAllUserSessionsForUser(userSession.UserId);

        await userRepository.AddUserSession(userSession);
        await userRepository.SaveChanges();

        var accessToken = authService.CreateAccessToken(user);
        var refreshToken = userSession.Token;

        return (accessToken, refreshToken);
    }
    
    public async Task<ErrorOr<TokensPair>> RefreshTokens(string expiredAccessToken, string refreshToken)
    {
        var userEmail = authService.GetEmailFromJwt(expiredAccessToken);

        if (userEmail.IsError)
            return userEmail.FirstError;
        
        var user = await userRepository.FindUserByEmail(userEmail.Value);

        if (user is null)
            return Errors.User.NotFound;

        var userSession = await userRepository.GetUserSession(user.UserId, refreshToken);

        if (userSession is null)
            return Errors.UserSession.InvalidToken;

        await userSession.Update();
        await userRepository.SaveChanges();

        var newAccessToken = authService.CreateAccessToken(user);
        var newRefreshToken = userSession.Token;

        return (newAccessToken, newRefreshToken);
    }

    private async Task<bool> AreThereTooManySessionsPerUser(Guid userId)
    {
        return await userRepository.GetNumberOfUserSessionsForUser(userId) >= UserSession.MaxSessionsPerUser;
    }
}