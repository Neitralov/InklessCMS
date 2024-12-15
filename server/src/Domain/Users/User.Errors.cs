namespace Domain.Users;

public sealed partial class User
{
    public static class Errors
    {
        public static Error NotFound => Error.NotFound(
            code: $"{nameof(User)}.{nameof(NotFound)}",
            description: "Пользователь не найден.");

        public static Error InvalidEmail => Error.Validation(
            code: $"{nameof(User)}.{nameof(InvalidEmail)}",
            description: "Email указан некорректно.");

        public static Error InvalidPassword => Error.Validation(
            code: $"{nameof(User)}.{nameof(InvalidPassword)}",
            description: $"Пароль должен быть не короче {MinPasswordLength} символов.");
        
        public static Error IncorrectEmailOrPassword => Error.Validation(
            code: $"{nameof(User)}.{nameof(IncorrectEmailOrPassword)}",
            description: "Электронная почта или пароль указаны неверно.");
    }
}