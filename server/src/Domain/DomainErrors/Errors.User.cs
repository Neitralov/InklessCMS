namespace Domain.DomainErrors;

public static partial class Errors
{
    public static class User
    {
        public static Error NotFound => Error.NotFound(
            code:        $"{nameof(Entities.User)}.{nameof(NotFound)}",
            description: "Пользователь не найден");
        
        public static Error InvalidEmail => Error.Validation(
            code:        $"{nameof(Entities.User)}.{nameof(InvalidEmail)}",
            description: "Email указан некорректно");
        
        public static Error InvalidPassword => Error.Validation(
            code:        $"{nameof(Entities.User)}.{nameof(InvalidPassword)}",
            description: $"Пароль должен быть не короче {Entities.User.MinPasswordLength} символов");
    }
}