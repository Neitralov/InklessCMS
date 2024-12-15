namespace Domain.Authorization;

public sealed class UserSession
{
    public long UserSessionId { get; private set; }
    public Guid UserId { get; private set; }
    public RefreshToken RefreshToken { get; private set; } = default!;
    public DateTime ExpirationDate { get; private set; }

    public const int ExpiresInDays = 30;
    public const int MaxSessionsPerUser = 5;

    private UserSession() { }

    public static UserSession Create(Guid userId)
    {
        var refreshToken = new RefreshToken(Guid.NewGuid().ToString());

        return new UserSession
        {
            UserId = userId,
            RefreshToken = refreshToken,
            ExpirationDate = DateTime.UtcNow.AddDays(ExpiresInDays)
        };
    }

    public Task Update()
    {
        RefreshToken = new RefreshToken(Guid.NewGuid().ToString());

        return Task.CompletedTask;
    }
}