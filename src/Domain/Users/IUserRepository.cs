namespace Domain.Users;

public interface IUserRepository
{
    Task AddUserSessionAsync(UserSession newUserSession);
    Task<ErrorOr<User>> FindUserByEmailAsync(string email);
    Task<int> GetNumberOfUserSessionsForUserAsync(Guid userId);
    Task<ErrorOr<UserSession>> GetUserSessionAsync(Guid userId, RefreshToken refreshToken);
    Task DeleteAllUserSessionsForUserAsync(Guid userId);
    Task SaveChangesAsync();
}
