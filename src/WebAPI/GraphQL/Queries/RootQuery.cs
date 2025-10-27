namespace WebAPI.GraphQL.Queries;

[GraphQLName("RootQuery")]
public sealed class RootQuery
{
    [GraphQLName("articleQueries")]
    [GraphQLDescription("Запросы к статьям")]
    public GqlArticleQueries ArticleQueries { get; } = new();

    [GraphQLName("collectionQueries")]
    [GraphQLDescription("Запросы к коллекциям")]
    public GqlCollectionQueries CollectionQueries { get; } = new();
}
