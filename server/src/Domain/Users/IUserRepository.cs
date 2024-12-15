namespace Domain.Users;

public interface IUserRepository
{
    Task AddUserSession(UserSession newUserSession);
    Task<ErrorOr<User>> FindUserByEmail(string email);
    Task<int> GetNumberOfUserSessionsForUser(Guid userId);
    Task<ErrorOr<UserSession>> GetUserSession(Guid userId, RefreshToken refreshToken);
    Task DeleteAllUserSessionsForUser(Guid userId);
    Task SaveChanges();
}