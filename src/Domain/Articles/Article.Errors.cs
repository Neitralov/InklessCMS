namespace Domain.Articles;

public sealed partial record Article
{
    public static class Errors
    {
        public static Error NotFound => Error.NotFound(
            code: $"{nameof(Article)}.{nameof(NotFound)}",
            description: "Статья не найдена.");

        public static Error NonUniqueId => Error.Validation(
            code: $"{nameof(Article)}.{nameof(NonUniqueId)}",
            description: "Статья с таким Id уже существует.");

        public static Error InvalidId => Error.Validation(
            code: $"{nameof(Article)}.{nameof(InvalidId)}",
            description: $"Id статьи должен соответствовать выражению {ArticleIdPattern}.");

        public static Error InvalidIdLength => Error.Validation(
            code: $"{nameof(Article)}.{nameof(InvalidIdLength)}",
            description: $"Id статьи не может быть короче {MinIdLength} символов " +
                $"и длиннее {MaxIdLength} символов.");

        public static Error InvalidTitleLength => Error.Validation(
            code: $"{nameof(Article)}.{nameof(InvalidTitleLength)}",
            description: $"Заголовок статьи не может быть короче {MinTitleLength} символов " +
                $"и длиннее {MaxTitleLength} символов.");

        public static Error InvalidDescriptionLength => Error.Validation(
            code: $"{nameof(Article)}.{nameof(InvalidDescriptionLength)}",
            description: $"Описание статьи не может быть длиннее {MaxDescriptionLength} символов.");
    }
}
