namespace Domain.Articles;

public static partial class Errors
{
    public static class Article
    {
        public static Error NotFound => Error.NotFound(
            code: $"{nameof(Articles.Article)}.{nameof(NotFound)}",
            description: "Статья не найдена.");

        public static Error NonUniqueId => Error.Validation(
            code: $"{nameof(Articles.Article)}.{nameof(NonUniqueId)}",
            description: "Статья с таким Id уже существует.");

        public static Error InvalidId => Error.Validation(
            code: $"{nameof(Articles.Article)}.{nameof(InvalidId)}",
            description: $"Id статьи должен соответствовать выражению {Articles.Article.ArticleIdPattern}.");

        public static Error InvalidIdLength => Error.Validation(
            code: $"{nameof(Articles.Article)}.{nameof(InvalidIdLength)}",
            description: $"Id статьи не может быть короче {Articles.Article.MinIdLength} символов " +
                         $"и длиннее {Articles.Article.MaxIdLength} символов.");

        public static Error InvalidTitleLength => Error.Validation(
            code: $"{nameof(Articles.Article)}.{nameof(InvalidTitleLength)}",
            description: $"Заголовок статьи не может быть короче {Articles.Article.MinTitleLength} символов " +
                         $"и длиннее {Articles.Article.MaxTitleLength} символов..");

        public static Error InvalidDescriptionLength => Error.Validation(
            code: $"{nameof(Articles.Article)}.{nameof(InvalidDescriptionLength)}",
            description: $"Описание статьи не может быть длиннее {Articles.Article.MaxDescriptionLength} символов.");
    }
}