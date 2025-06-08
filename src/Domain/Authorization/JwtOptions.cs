namespace Domain.Authorization;

public sealed record JwtOptions
{
    public const string Section = "Jwt";

    [Required]
    [MinLength(64)]
    public string SecretKey { get; set; } = string.Empty;

    [Required]
    public int AccessTokenLifeTimeInMinutes { get; set; }
}
