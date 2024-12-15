namespace Domain.Users;

public sealed partial class User
{
    public Guid UserId { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public byte[] PasswordHash { get; private init; } = default!;
    public byte[] PasswordSalt { get; private init; } = default!;

    public bool CanManageArticles { get; private set; }

    public const int MinPasswordLength = 4;

    private User() { }

    public static ErrorOr<User> Create(string email, string password, bool canManageArticles = false)
    {
        List<Error> errors = [];

        if (email.Contains('@') is false)
            errors.Add(Errors.InvalidEmail);

        if (password.Trim().Length < MinPasswordLength)
            errors.Add(Errors.InvalidPassword);

        if (errors.Count > 0)
            return errors;

        CreatePasswordHash(password.Trim(), out var passwordHash, out var passwordSalt);

        return new User
        {
            UserId = Guid.NewGuid(),
            Email = email.Trim(),
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            CanManageArticles = canManageArticles
        };
    }

    private static void CreatePasswordHash(
        string password,
        out byte[] passwordHash,
        out byte[] passwordSalt)
    {
        using var hmac = new HMACSHA512();

        passwordSalt = hmac.Key;
        passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
    }

    public bool VerifyPasswordHash(string password)
    {
        using var hmac = new HMACSHA512(PasswordSalt);

        var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        return computedHash.SequenceEqual(PasswordHash);
    }
}