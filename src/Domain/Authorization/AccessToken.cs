namespace Domain.Authorization;

public sealed partial record AccessToken
{
    public string Token { get; private set; } = string.Empty;

    private AccessToken() { }

    public static AccessToken Create(User user, IOptions<JwtOptions> options)
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

        return new AccessToken
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token)
        };
    }

    public static ErrorOr<string> GetEmailFromRawToken(string token, IOptions<JwtOptions> options)
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
            var claimsPrincipal = tokenHandler.ValidateToken(token, tokenValidationParameters, out _);
            return claimsPrincipal.Identity?.Name!;
        }
        catch
        {
            return AccessToken.Errors.InvalidToken;
        }
    }
}
