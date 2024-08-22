namespace Domain.Options;

/// <summary>Объект для типизированной конфигурации учетной записи администратора</summary>
public class AdminAccountOptions
{
    /// <summary>Название секции в файле конфигурации</summary>
    public const string Section = "Admin";

    /// <summary>Электронная почта администратора</summary>
    [Required]
    public string Email { get; set; } = string.Empty;
    
    /// <summary>Пароль администратора</summary>
    [Required]
    public string Password { get; set; } = string.Empty;
}