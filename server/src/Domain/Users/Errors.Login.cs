namespace Domain.Users;

public static partial class Errors
{
    public static class Login
    {
        public static Error IncorrectEmailOrPassword => Error.Validation(
            code: $"{nameof(Login)}.{nameof(IncorrectEmailOrPassword)}",
            description: "Электронная почта или пароль указаны неверно.");
    }
}