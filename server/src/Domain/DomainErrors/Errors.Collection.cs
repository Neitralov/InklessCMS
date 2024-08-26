namespace Domain.DomainErrors;

public static partial class Errors
{
    public static class Collection
    {
        public static Error NotFound => Error.NotFound(
            code: $"{nameof(Entities.Collection)}.{nameof(NotFound)}",
            description: "Коллекция не найдена");

        public static Error ArticleNotFound => Error.NotFound(
            code: $"{nameof(Entities.Collection)}.{nameof(ArticleNotFound)}",
            description: "Статья в коллекции не найдена");

        public static Error NonUniqueId => Error.Validation(
            code: $"{nameof(Entities.Collection)}.{nameof(NonUniqueId)}",
            description: "Коллекция с таким Id уже существует");

        public static Error InvalidId => Error.Validation(
            code: $"{nameof(Entities.Collection)}.{nameof(InvalidId)}",
            description: $"Id коллекции должен соответствовать регулярному выражению {Entities.Collection.CollectionIdPattern}");

        public static Error InvalidIdLength => Error.Validation(
            code: $"{nameof(Entities.Collection)}.{nameof(InvalidIdLength)}",
            description: $"Id коллекции не может быть короче {Entities.Collection.MinIdLength} символов и длиннее {Entities.Collection.MaxIdLength} символов");

        public static Error InvalidTitleLength => Error.Validation(
            code: $"{nameof(Entities.Collection)}.{nameof(InvalidTitleLength)}",
            description: $"Название коллекции не может быть короче {Entities.Collection.MinTitleLength} символов и длиннее {Entities.Collection.MaxTitleLength} символов");

        public static Error ArticleAlreadyAdded => Error.Validation(
            code: $"{nameof(Entities.Collection)}.{nameof(ArticleAlreadyAdded)}",
            description: "Статья уже находилась в коллекции");
    }
}
