namespace Domain.Authorization;

/// <summary>Объект для типизированной конфигурации учетной записи администратора</summary>
public sealed class JwtOptions
{
    /// <summary>Название секции в файле конфигурации</summary>
    public const string Section = "Jwt";

    /// <summary>Секретный ключ для подписи JWT токенов</summary>
    [Required]
    [MinLength(64)]
    public string SecretKey { get; set; } = string.Empty;

    /// <summary>Время жизни access токена</summary>
    [Required]
    public int AccessTokenLifeTimeInMinutes { get; set; }
}