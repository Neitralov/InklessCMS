namespace Domain.DomainErrors;

public static partial class Errors
{
    public static class AccessToken
    {
        public static Error NotFound => Error.NotFound(
            code:        $"{nameof(AccessToken)}.{nameof(NotFound)}",
            description: "Токен доступа не найден");

        public static Error InvalidToken => Error.Validation(
            code:        $"{nameof(AccessToken)}.{nameof(InvalidToken)}",
            description: "Токен доступа недействителен");
    }
}