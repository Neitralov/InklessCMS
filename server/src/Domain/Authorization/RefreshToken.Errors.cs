namespace Domain.Authorization;

public sealed partial record RefreshToken
{
    public static class Errors
    {
        public static Error NotFound => Error.NotFound(
            code: $"{nameof(RefreshToken)}.{nameof(NotFound)}",
            description: "Токен обновления не найден.");

        public static Error InvalidToken => Error.Validation(
            code: $"{nameof(RefreshToken)}.{nameof(InvalidToken)}",
            description: "Токен обновления недействителен.");
    }
}
