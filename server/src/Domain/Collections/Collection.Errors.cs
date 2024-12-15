namespace Domain.Collections;

public sealed partial class Collection
{
    public static class Errors
    {
        public static Error NotFound => Error.NotFound(
            code: $"{nameof(Collection)}.{nameof(NotFound)}",
            description: "Коллекция не найдена.");

        public static Error ArticleNotFound => Error.NotFound(
            code: $"{nameof(Collection)}.{nameof(ArticleNotFound)}",
            description: "Статья в коллекции не найдена.");

        public static Error NonUniqueId => Error.Validation(
            code: $"{nameof(Collection)}.{nameof(NonUniqueId)}",
            description: "Коллекция с таким Id уже существует.");

        public static Error InvalidId => Error.Validation(
            code: $"{nameof(Collection)}.{nameof(InvalidId)}",
            description: $"Id коллекции должен соответствовать выражению " +
                         $"{CollectionIdPattern}.");

        public static Error InvalidIdLength => Error.Validation(
            code: $"{nameof(Collection)}.{nameof(InvalidIdLength)}",
            description: $"Id коллекции не может быть короче {MinIdLength} символов и длиннее {MaxIdLength} символов.");

        public static Error InvalidTitleLength => Error.Validation(
            code: $"{nameof(Collection)}.{nameof(InvalidTitleLength)}",
            description: $"Название коллекции не может быть короче {MinTitleLength} символов " +
                         $"и длиннее {MaxTitleLength} символов.");

        public static Error ArticleAlreadyAdded => Error.Validation(
            code: $"{nameof(Collection)}.{nameof(ArticleAlreadyAdded)}",
            description: "Статья уже в коллекции.");
    }
}