namespace Domain.Interfaces;

public interface IAuthService
{
    string CreateAccessToken(User user);
    ErrorOr<string> GetEmailFromJwt(string expiredAccessToken);
}