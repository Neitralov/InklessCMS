namespace WebAPI.GraphQL.Mutations;

[GraphQLName("UserMutations")]
public sealed class GqlUserMutations
{
    [GraphQLName("login")]
    [GraphQLDescription("Войти в аккаунт")]
    public async Task<GqlTokens> LoginAsync(
        [Service] IUserRepository userRepository,
        [Service] IOptions<JwtOptions> jwtOptions,
        LoginUserRequest request)
    {
        var user = await userRepository.FindUserByEmailAsync(request.Email);

        if (user.Errors.Contains(User.Errors.NotFound))
            throw new Exception(User.Errors.IncorrectEmailOrPassword.Code);

        if (user.Value.VerifyPasswordHash(request.Password) is false)
            throw new Exception(User.Errors.IncorrectEmailOrPassword.Code);

        var userSession = UserSession.Create(user.Value.UserId);

        if (await AreThereTooManySessionsPerUserAsync(userSession.UserId))
            await userRepository.DeleteAllUserSessionsForUserAsync(userSession.UserId);

        await userRepository.AddUserSessionAsync(userSession);
        await userRepository.SaveChangesAsync();

        var accessToken = AccessToken.Create(user.Value, jwtOptions);

        return new GqlTokens(accessToken, userSession.RefreshToken);


        async Task<bool> AreThereTooManySessionsPerUserAsync(Guid userId)
        {
            return await userRepository.GetNumberOfUserSessionsForUserAsync(userId) >= UserSession.MaxSessionsPerUser;
        }
    }

    [GraphQLName("refreshTokens")]
    [GraphQLDescription("Обновить access и refresh токены")]
    public async Task<GqlTokens> RefreshTokensAsync(
        [Service] IUserRepository userRepository,
        [Service] IOptions<JwtOptions> jwtOptions,
        RefreshUserTokensRequest request)
    {
        var userEmail = AccessToken.GetEmailFromRawToken(request.ExpiredAccessToken, jwtOptions);

        if (userEmail.IsError)
            throw new Exception(userEmail.FirstError.Code);

        var user = await userRepository.FindUserByEmailAsync(userEmail.Value);

        if (user.IsError)
            throw new Exception(user.FirstError.Code);

        var refreshToken = new RefreshToken(request.RefreshToken);
        var userSession = await userRepository.GetUserSessionAsync(user.Value.UserId, refreshToken);

        if (userSession.IsError)
            throw new Exception(userSession.FirstError.Code);

        userSession.Value.UpdateRefreshToken();
        await userRepository.SaveChangesAsync();

        var newAccessToken = AccessToken.Create(user.Value, jwtOptions);

        return new GqlTokens(newAccessToken, userSession.Value.RefreshToken);
    }
}
