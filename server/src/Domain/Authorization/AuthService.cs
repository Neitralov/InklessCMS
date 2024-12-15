namespace Domain.Authorization;

public interface IAuthService
{
    AccessToken CreateAccessToken(User user);
    ErrorOr<string> GetEmailFromJwt(AccessToken expiredAccessToken);
}

public sealed class AuthService(IOptions<JwtOptions> options) : IAuthService
{
    public AccessToken CreateAccessToken(User user)
    {
        var claims = new List<Claim>
        {
            new (ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new (ClaimTypes.Name, user.Email),
            new (nameof(User.CanManageArticles), user.CanManageArticles.ToString())
        };

        var configToken = options.Value.SecretKey;
        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(configToken));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(options.Value.AccessTokenLifeTimeInMinutes),
            signingCredentials: creds);
        
        return new AccessToken(new JwtSecurityTokenHandler().WriteToken(token));
    }

    public ErrorOr<string> GetEmailFromJwt(AccessToken expiredAccessToken)
    {
        var configToken = options.Value.SecretKey;
        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(configToken));

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ValidateLifetime = false
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        try
        {
            var claimsPrincipal = 
                tokenHandler.ValidateToken(expiredAccessToken.Token, tokenValidationParameters, out _);
            return claimsPrincipal.Identity?.Name!;
        }
        catch
        {
            return AccessToken.Errors.InvalidToken;
        }
    }
}