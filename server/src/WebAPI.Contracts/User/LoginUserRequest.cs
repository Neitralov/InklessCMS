namespace WebAPI.Contracts.User;

public sealed record LoginUserRequest(
    string Email,
    string Password)
{
    /// <example>admin@example.ru</example>
    public string Email { get; init; } = Email;
    /// <example>admin</example>
    public string Password { get; init; } = Password;
}
