namespace Domain.DomainErrors;

public static partial class Errors
{
    public static class Article
    {
        public static Error NotFound => Error.NotFound(
            code:        $"{nameof(Entities.Article)}.{nameof(NotFound)}",
            description: "Статья не найдена");

        public static Error NonUniqueId => Error.Validation(
            code:        $"{nameof(Entities.Article)}.{nameof(NonUniqueId)}",
            description: "Статья с таким Id уже существует");
        
        public static Error InvalidId => Error.Validation(
            code:        $"{nameof(Entities.Article)}.{nameof(InvalidId)}",
            description: $"Id статьи должен соответствовать регулярному выражению {Entities.Article.ArticleIdPattern}");
        
        public static Error InvalidIdLength => Error.Validation(
            code:        $"{nameof(Entities.Article)}.{nameof(InvalidIdLength)}",
            description: $"Id статьи не может быть короче {Entities.Article.MinIdLength} символов и длиннее {Entities.Article.MaxIdLength} символов");
        
        public static Error InvalidTitleLength => Error.Validation(
            code:        $"{nameof(Entities.Article)}.{nameof(InvalidTitleLength)}",
            description: $"Заголовок статьи не может быть короче {Entities.Article.MinTitleLength} символов и длиннее {Entities.Article.MaxTitleLength} символов");

        public static Error InvalidDescriptionLength => Error.Validation(
            code:        $"{nameof(Entities.Article)}.{nameof(InvalidDescriptionLength)}",
            description: $"Описание статьи не может быть длиннее {Entities.Article.MaxDescriptionLength} символов");
    }
}
