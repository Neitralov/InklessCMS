namespace Database.Repositories;

public class UserRepository(DatabaseContext database) : IUserRepository
{
    public async Task AddUserSession(UserSession newUserSession)
    {
        await database.AddAsync(newUserSession);
    }

    public async Task<ErrorOr<User>> FindUserByEmail(string email)
    {
        var user = await database.Users.SingleOrDefaultAsync(user => user.Email == email);

        return user is null ? Errors.User.NotFound : user;
    }

    public async Task<int> GetNumberOfUserSessionsForUser(Guid userId)
    {
        return await database.UserSessions.CountAsync(refreshTokenSession => refreshTokenSession.UserId == userId);
    }

    public async Task<ErrorOr<UserSession>> GetUserSession(Guid userId, string refreshToken)
    {
        var userSession = await database.UserSessions.SingleOrDefaultAsync(refreshTokenSession =>
            refreshTokenSession.UserId == userId &&
            refreshTokenSession.Token == refreshToken &&
            refreshTokenSession.ExpirationDate >= DateTime.UtcNow);

        return userSession is null ? Errors.UserSession.NotFound : userSession;
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
