namespace Database.Repositories;

public sealed class UserRepository(DatabaseContext database) : BaseRepository(database), IUserRepository
{
    private readonly DatabaseContext _database = database;

    public async Task AddUserSession(UserSession newUserSession) => await _database.AddAsync(newUserSession);

    public async Task<ErrorOr<User>> FindUserByEmail(string email) =>
        await _database.Users.SingleOrDefaultAsync(user => user.Email == email) ??
        User.Errors.NotFound.ToErrorOr<User>();

    public async Task<int> GetNumberOfUserSessionsForUser(Guid userId) =>
        await _database.UserSessions.CountAsync(refreshTokenSession => refreshTokenSession.UserId == userId);

    public async Task<ErrorOr<UserSession>> GetUserSession(Guid userId, string refreshToken) =>
        await _database.UserSessions.SingleOrDefaultAsync(refreshTokenSession =>
            refreshTokenSession.UserId == userId &&
            refreshTokenSession.Token == refreshToken &&
            refreshTokenSession.ExpirationDate >= DateTime.UtcNow) ??
        UserSession.Errors.NotFound.ToErrorOr<UserSession>();

    public async Task DeleteAllUserSessionsForUser(Guid userId)
    {
        var usersRefreshTokenSessions = await _database.UserSessions
            .Where(refreshTokenSession => refreshTokenSession.UserId == userId)
            .ToListAsync();

        _database.UserSessions.RemoveRange(usersRefreshTokenSessions);
    }

    public override async Task SaveChanges()
    {
        await DeleteAllInvalidRefreshTokenSessions();
        await _database.SaveChangesAsync();
    }

    private async Task DeleteAllInvalidRefreshTokenSessions()
    {
        var invalidSessions = await _database.UserSessions
            .Where(session => session.ExpirationDate < DateTime.UtcNow)
            .ToListAsync();

        _database.RemoveRange(invalidSessions);
    }
}