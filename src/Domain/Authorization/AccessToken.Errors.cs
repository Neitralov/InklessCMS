namespace Domain.Authorization;

public sealed partial record AccessToken
{
    public static class Errors
    {
        public static Error NotFound => Error.NotFound(
            code: $"{nameof(AccessToken)}.{nameof(NotFound)}",
            description: "Токен доступа не найден.");

        public static Error InvalidToken => Error.Validation(
            code: $"{nameof(AccessToken)}.{nameof(InvalidToken)}",
            description: "Токен доступа недействителен.");
    }
}
