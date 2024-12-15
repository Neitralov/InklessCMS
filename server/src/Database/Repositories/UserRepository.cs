namespace Database.Repositories;

public sealed class UserRepository(DatabaseContext database) : BaseRepository(database), IUserRepository
{
    private readonly DatabaseContext _database = database;

    public async Task AddUserSession(UserSession newUserSession) => await _database.AddAsync(newUserSession);

    public async Task<ErrorOr<User>> FindUserByEmail(string email) =>
        await _database.Users.SingleOrDefaultAsync(user => user.Email == email) ??
        User.Errors.NotFound.ToErrorOr<User>();

    public async Task<int> GetNumberOfUserSessionsForUser(Guid userId) =>
        await _database.UserSessions.CountAsync(userSession => userSession.UserId == userId);

    public async Task<ErrorOr<UserSession>> GetUserSession(Guid userId, RefreshToken refreshToken) =>
        await _database.UserSessions.SingleOrDefaultAsync(userSession =>
            userSession.UserId == userId &&
            userSession.RefreshToken == refreshToken &&
            userSession.ExpirationDate >= DateTime.UtcNow) ??
        RefreshToken.Errors.NotFound.ToErrorOr<UserSession>();

    public async Task DeleteAllUserSessionsForUser(Guid userId)
    {
        var usersSessions = await _database.UserSessions
            .Where(userSession => userSession.UserId == userId)
            .ToListAsync();

        _database.UserSessions.RemoveRange(usersSessions);
    }

    public override async Task SaveChanges()
    {
        await DeleteAllInvalidUserSessions();
        await _database.SaveChangesAsync();
    }

    private async Task DeleteAllInvalidUserSessions()
    {
        var invalidUserSessions = await _database.UserSessions
            .Where(userSession => userSession.ExpirationDate < DateTime.UtcNow)
            .ToListAsync();

        _database.RemoveRange(invalidUserSessions);
    }
}