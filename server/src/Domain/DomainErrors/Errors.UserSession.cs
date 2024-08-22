namespace Domain.DomainErrors;

public static partial class Errors
{
    public static class UserSession
    {
        public static Error NotFound => Error.NotFound(
            code:        $"{nameof(Entities.UserSession)}.{nameof(NotFound)}",
            description: "Токен обновления не найден");
        
        public static Error InvalidToken => Error.Validation(
            code:        $"{nameof(Entities.UserSession)}.{nameof(InvalidToken)}",
            description: "Токен обновления недействителен");
    }
}