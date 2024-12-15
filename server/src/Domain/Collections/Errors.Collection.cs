namespace Domain.Collections;

public static partial class Errors
{
    public static class Collection
    {
        public static Error NotFound => Error.NotFound(
            code: $"{nameof(Collections.Collection)}.{nameof(NotFound)}",
            description: "Коллекция не найдена.");

        public static Error ArticleNotFound => Error.NotFound(
            code: $"{nameof(Collections.Collection)}.{nameof(ArticleNotFound)}",
            description: "Статья в коллекции не найдена.");

        public static Error NonUniqueId => Error.Validation(
            code: $"{nameof(Collections.Collection)}.{nameof(NonUniqueId)}",
            description: "Коллекция с таким Id уже существует.");

        public static Error InvalidId => Error.Validation(
            code: $"{nameof(Collections.Collection)}.{nameof(InvalidId)}",
            description: $"Id коллекции должен соответствовать выражению " +
                         $"{Collections.Collection.CollectionIdPattern}.");

        public static Error InvalidIdLength => Error.Validation(
            code: $"{nameof(Collections.Collection)}.{nameof(InvalidIdLength)}",
            description: $"Id коллекции не может быть короче {Collections.Collection.MinIdLength} символов " +
                         $"и длиннее {Collections.Collection.MaxIdLength} символов.");

        public static Error InvalidTitleLength => Error.Validation(
            code: $"{nameof(Collections.Collection)}.{nameof(InvalidTitleLength)}",
            description: $"Название коллекции не может быть короче {Collections.Collection.MinTitleLength} символов " +
                         $"и длиннее {Collections.Collection.MaxTitleLength} символов.");

        public static Error ArticleAlreadyAdded => Error.Validation(
            code: $"{nameof(Collections.Collection)}.{nameof(ArticleAlreadyAdded)}",
            description: "Статья уже в коллекции.");
    }
}