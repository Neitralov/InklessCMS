namespace WebAPI.Controllers;

[Route("/api/users"), Tags("Users")]
public sealed class UsersController(UserService userService) : ApiController
{
    /// <summary>Войти в аккаунт</summary>
    /// <response code="200">Вход произведен успешно</response>
    /// <response code="400">Логин или пароль указан некорректно</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginUserResponse), 200)]
    public async Task<IActionResult> Login([Required] LoginUserRequest request)
    {
        var loginUserResult = await userService.Login(request.Email, request.Password);

        if (loginUserResult.IsError)
            return Problem(loginUserResult.Errors);

        var accessToken = loginUserResult.Value.AccessToken;
        var refreshToken = loginUserResult.Value.RefreshToken;

        return Ok(new LoginUserResponse(accessToken.Token, refreshToken.Token));
    }

    /// <summary>Обновить access и refresh токены</summary>
    /// <response code="200">Токены обновлены успешно</response>
    /// <response code="400">
    /// Срок действия refresh токена истек; Refresh токен недействителен; Access токен недействителен
    /// </response>
    /// <response code="404">Владелец токена (пользователь) не найден</response>
    [HttpPost("refresh-tokens")]
    [ProducesResponseType(typeof(LoginUserResponse), 200)]
    public async Task<IActionResult> RefreshTokens([Required] RefreshUserTokensRequest request)
    {
        var refreshTokensResult = await userService.RefreshTokens(
            expiredAccessToken: new AccessToken(request.ExpiredAccessToken),
            refreshToken: new RefreshToken(request.RefreshToken));

        if (refreshTokensResult.IsError)
            return Problem(refreshTokensResult.Errors);

        var newAccessToken = refreshTokensResult.Value.AccessToken;
        var newRefreshToken = refreshTokensResult.Value.RefreshToken;

        return Ok(new LoginUserResponse(newAccessToken.Token, newRefreshToken.Token));
    }
}
