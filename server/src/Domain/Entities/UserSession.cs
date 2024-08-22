namespace Domain.Entities;

public class UserSession
{
    public long UserSessionId { get; private set; }
    public Guid UserId { get; private set; }
    public string Token { get; private set; } = string.Empty;
    public DateTime ExpirationDate { get; private set; }
    
    public const int ExpiresInDays = 30;
    public const int MaxSessionsPerUser = 5;
    
    private UserSession() { }
    
    public static UserSession Create(Guid userId)
    {
        var refreshToken = Guid.NewGuid().ToString();
        
        return new UserSession
        {
            UserId = userId,
            Token = refreshToken,
            ExpirationDate = DateTime.UtcNow.AddDays(ExpiresInDays)
        };
    }
    
    public Task Update()
    {
        Token = Guid.NewGuid().ToString();
        ExpirationDate = DateTime.UtcNow.AddDays(ExpiresInDays);

        return Task.CompletedTask;
    }
}