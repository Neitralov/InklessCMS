namespace WebAPI.GraphQL.OutputTypes;

[GraphQLName("articleList")]
public sealed record GqlArticleList(
    IEnumerable<GqlArticle> Articles,
    int TotalCount)
{
    [GraphQLName("articles")]
    [GraphQLDescription("some-article-id")]
    public IEnumerable<GqlArticle> Articles { get; init; } = Articles;

    [GraphQLName("totalCount")]
    [GraphQLDescription("10")]
    public int TotalCount { get; init; } = TotalCount;
}
