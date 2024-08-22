namespace Database.Repositories;

public class UserRepository(DatabaseContext database) : IUserRepository
{
    public async Task AddUserSession(UserSession newUserSession)
    {
        await database.AddAsync(newUserSession);
    }

    public async Task<User?> FindUserByEmail(string email)
    {
        return await database.Users.SingleOrDefaultAsync(user => user.Email == email);
    }

    public async Task<int> GetNumberOfUserSessionsForUser(Guid userId)
    {
        return await database.UserSessions.CountAsync(refreshTokenSession => refreshTokenSession.UserId == userId);
    }

    public async Task<UserSession?> GetUserSession(Guid userId, string refreshToken)
    {
        return await database.UserSessions.SingleOrDefaultAsync(refreshTokenSession =>
            refreshTokenSession.UserId == userId &&
            refreshTokenSession.Token == refreshToken &&
            refreshTokenSession.ExpirationDate >= DateTime.UtcNow);
    }

    public async Task DeleteAllUserSessionsForUser(Guid userId)
    {
        var usersRefreshTokenSessions = await database.UserSessions.Where(refreshTokenSession => refreshTokenSession.UserId == userId).ToListAsync();
        database.UserSessions.RemoveRange(usersRefreshTokenSessions);
    }
    
    public async Task SaveChanges()
    {
        await DeleteAllInvalidRefreshTokenSessions();
        await database.SaveChangesAsync();
    }

    private async Task DeleteAllInvalidRefreshTokenSessions()
    {
        var invalidSessions = await database.UserSessions.Where(session => session.ExpirationDate < DateTime.UtcNow).ToListAsync();
        database.RemoveRange(invalidSessions);
    }
}