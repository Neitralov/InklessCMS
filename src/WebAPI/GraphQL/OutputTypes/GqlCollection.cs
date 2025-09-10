namespace WebAPI.GraphQL.OutputTypes;

[GraphQLName("collection")]
public sealed class GqlCollection(Collection collection)
{
    [GraphQLName("collectionId")]
    [GraphQLDescription("some-collection-id")]
    public string CollectionId { get; init; } = collection.CollectionId;

    [GraphQLName("title")]
    [GraphQLDescription("Название коллекции")]
    public string Title { get; init; } = collection.Title;

    [GraphQLName("articles")]
    [GraphQLDescription("Статьи коллекции")]
    public IEnumerable<GqlArticle>? Articles { get; init; } = collection.Articles.Select(article =>
        article.ToGqlArticle());
}
