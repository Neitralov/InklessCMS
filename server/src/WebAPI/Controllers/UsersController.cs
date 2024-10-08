namespace WebAPI.Controllers;

/// <inheritdoc />
[Route("/api/users"), Tags("Users")]
public class UsersController(UserService userService) : ApiController
{
    /// <summary>Войти в аккаунт</summary>
    /// <response code="200">Вход произведен успешно</response>
    /// <response code="400">Логин или пароль указан некорректно</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginUserResponse), 200)]
    public async Task<IActionResult> Login([Required] LoginUserRequest request)
    {
        ErrorOr<TokensPair> loginUserResult = await userService.Login(request.Email, request.Password);

        if (loginUserResult.IsError)
            return Problem(loginUserResult.Errors);

        var accessToken = loginUserResult.Value.AccessToken;
        var refreshToken = loginUserResult.Value.RefreshToken;

        return Ok(new LoginUserResponse(accessToken, refreshToken));
    }

    /// <summary>Обновить access и refresh токены</summary>
    /// <response code="200">Токены обновлены успешно</response>
    /// <response code="400">Срок действия refresh токена истек; Refresh токен недействителен; Access токен недействителен</response>
    /// <response code="404">Владелец токена (пользователь) не найден</response>
    [HttpPost("refresh-tokens")]
    [ProducesResponseType(typeof(LoginUserResponse), 200)]
    public async Task<IActionResult> RefreshTokens([Required] RefreshUserTokensRequest request)
    {
        ErrorOr<TokensPair> refreshTokensResult = await userService.RefreshTokens(request.ExpiredAccessToken, request.RefreshToken);

        if (refreshTokensResult.IsError)
            return Problem(refreshTokensResult.Errors);

        var newAccessToken = refreshTokensResult.Value.AccessToken;
        var newRefreshToken = refreshTokensResult.Value.RefreshToken;
        
        return Ok(new LoginUserResponse(newAccessToken, newRefreshToken));
    }
}