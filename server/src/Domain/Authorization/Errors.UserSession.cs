namespace Domain.Authorization;

public static partial class Errors
{
    public static class UserSession
    {
        public static Error NotFound => Error.NotFound(
            code: $"{nameof(Authorization.UserSession)}.{nameof(NotFound)}",
            description: "Токен обновления не найден.");

        public static Error InvalidToken => Error.Validation(
            code: $"{nameof(Authorization.UserSession)}.{nameof(InvalidToken)}",
            description: "Токен обновления недействителен.");
    }
}