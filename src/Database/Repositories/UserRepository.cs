namespace Database.Repositories;

public sealed class UserRepository(DatabaseContext database) : BaseRepository(database), IUserRepository
{
    private readonly DatabaseContext _database = database;
    private readonly DbSet<User> _users = database.Set<User>();
    private readonly DbSet<UserSession> _userSessions = database.Set<UserSession>();

    public async Task AddUserSessionAsync(UserSession newUserSession) => await _userSessions.AddAsync(newUserSession);

    public async Task<ErrorOr<User>> FindUserByEmailAsync(string email)
    {
        return await _users.SingleOrDefaultAsync(user => user.Email == email) ??
            User.Errors.NotFound.ToErrorOr<User>();
    }

    public async Task<int> GetNumberOfUserSessionsForUserAsync(Guid userId)
    {
        return await _userSessions.CountAsync(userSession => userSession.UserId == userId);
    }

    public async Task<ErrorOr<UserSession>> GetUserSessionAsync(Guid userId, RefreshToken refreshToken)
    {
        return await _userSessions.SingleOrDefaultAsync(userSession =>
            userSession.UserId == userId &&
            userSession.RefreshToken == refreshToken &&
            userSession.ExpirationDate >= DateTime.UtcNow) ??
            RefreshToken.Errors.NotFound.ToErrorOr<UserSession>();
    }

    public async Task DeleteAllUserSessionsForUserAsync(Guid userId)
    {
        var usersSessions = await _userSessions
            .Where(userSession => userSession.UserId == userId)
            .ToListAsync();

        _userSessions.RemoveRange(usersSessions);
    }

    public override async Task SaveChangesAsync()
    {
        await DeleteAllInvalidUserSessionsAsync();
        await _database.SaveChangesAsync();
    }

    private async Task DeleteAllInvalidUserSessionsAsync()
    {
        var invalidUserSessions = await _userSessions
            .Where(userSession => userSession.ExpirationDate < DateTime.UtcNow)
            .ToListAsync();

        _userSessions.RemoveRange(invalidUserSessions);
    }
}
