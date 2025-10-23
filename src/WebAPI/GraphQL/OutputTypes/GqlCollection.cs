namespace WebAPI.GraphQL.OutputTypes;

[GraphQLName("collection")]
public sealed class GqlCollection
{
    [GraphQLName("collectionId")]
    [GraphQLDescription("some-collection-id")]
    public string CollectionId { get; init; }

    [GraphQLName("title")]
    [GraphQLDescription("Название коллекции")]
    public string Title { get; init; }

    [GraphQLName("articles")]
    [GraphQLDescription("Статьи коллекции")]
    public IEnumerable<GqlArticle>? Articles { get; init; }
}

public static class GqlCollectionExtensions
{
    public static GqlCollection ToGqlCollection(this Collection collection) => new()
    {
        CollectionId = collection.CollectionId,
        Title = collection.Title,
        Articles = collection.Articles.Select(article => article.ToGqlArticle())
    };
}

