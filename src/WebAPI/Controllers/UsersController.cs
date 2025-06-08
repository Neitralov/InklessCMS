namespace WebAPI.Controllers;

[Route("/api/users"), Tags("Users")]
public sealed class UsersController(
    IUserRepository userRepository,
    IOptions<JwtOptions> jwtOptions)
    : ApiController
{
    /// <summary>Войти в аккаунт</summary>
    /// <response code="200">Вход произведен успешно</response>
    /// <response code="400">Логин или пароль указан некорректно</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginUserResponse), 200)]
    public async Task<IActionResult> LoginAsync([Required] LoginUserRequest request)
    {
        var user = await userRepository.FindUserByEmailAsync(request.Email);

        if (user.Errors.Contains(Domain.Users.User.Errors.NotFound))
            return Problem([ Domain.Users.User.Errors.IncorrectEmailOrPassword ]);

        if (user.Value.VerifyPasswordHash(request.Password) is false)
            return Problem([ Domain.Users.User.Errors.IncorrectEmailOrPassword ]);

        var userSession = UserSession.Create(user.Value.UserId);

        if (await AreThereTooManySessionsPerUserAsync(userSession.UserId))
            await userRepository.DeleteAllUserSessionsForUserAsync(userSession.UserId);

        await userRepository.AddUserSessionAsync(userSession);
        await userRepository.SaveChangesAsync();

        var accessToken = AccessToken.Create(user.Value, jwtOptions);

        return Ok(new LoginUserResponse(accessToken.Token, userSession.RefreshToken.Token));


        async Task<bool> AreThereTooManySessionsPerUserAsync(Guid userId)
        {
            return await userRepository.GetNumberOfUserSessionsForUserAsync(userId) >= UserSession.MaxSessionsPerUser;
        }
    }

    /// <summary>Обновить access и refresh токены</summary>
    /// <response code="200">Токены обновлены успешно</response>
    /// <response code="400">
    /// Срок действия refresh токена истек; Refresh токен недействителен; Access токен недействителен
    /// </response>
    /// <response code="404">Владелец токена (пользователь) не найден</response>
    [HttpPost("refresh-tokens")]
    [ProducesResponseType(typeof(LoginUserResponse), 200)]
    public async Task<IActionResult> RefreshTokensAsync([Required] RefreshUserTokensRequest request)
    {
        var userEmail = AccessToken.GetEmailFromRawToken(request.ExpiredAccessToken, jwtOptions);

        if (userEmail.IsError)
            return Problem(userEmail.Errors);

        var user = await userRepository.FindUserByEmailAsync(userEmail.Value);

        if (user.IsError)
            return Problem(user.Errors);

        var refreshToken = new RefreshToken(request.RefreshToken);
        var userSession = await userRepository.GetUserSessionAsync(user.Value.UserId, refreshToken);

        if (userSession.IsError)
            return Problem(userSession.Errors);

        userSession.Value.UpdateRefreshToken();
        await userRepository.SaveChangesAsync();

        var newAccessToken = AccessToken.Create(user.Value, jwtOptions);

        return Ok(new LoginUserResponse(newAccessToken.Token, userSession.Value.RefreshToken.Token));
    }
}
