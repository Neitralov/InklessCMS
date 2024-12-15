namespace Domain.Users;

public static partial class Errors
{
    public static class User
    {
        public static Error NotFound => Error.NotFound(
            code: $"{nameof(Users.User)}.{nameof(NotFound)}",
            description: "Пользователь не найден.");

        public static Error InvalidEmail => Error.Validation(
            code: $"{nameof(Users.User)}.{nameof(InvalidEmail)}",
            description: "Email указан некорректно.");

        public static Error InvalidPassword => Error.Validation(
            code: $"{nameof(Users.User)}.{nameof(InvalidPassword)}",
            description: $"Пароль должен быть не короче {Users.User.MinPasswordLength} символов.");
    }
}