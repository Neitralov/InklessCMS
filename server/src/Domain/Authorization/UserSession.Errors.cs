namespace Domain.Authorization;

public sealed partial class UserSession
{
    public static class Errors
    {
        public static Error NotFound => Error.NotFound(
            code: $"{nameof(UserSession)}.{nameof(NotFound)}",
            description: "Токен обновления не найден.");

        public static Error InvalidToken => Error.Validation(
            code: $"{nameof(UserSession)}.{nameof(InvalidToken)}",
            description: "Токен обновления недействителен.");
    }
}