namespace WebAPI.GraphQL.InputTypes;

[GraphQLName("articleInput")]
public sealed record GqlArticleInput
{
    [GraphQLName("articleId")]
    [GraphQLDescription("some-article-id")]
    public string ArticleId { get; init; } = string.Empty;

    [GraphQLName("title")]
    [GraphQLDescription("Заголовок статьи")]
    public string Title { get; init; } = string.Empty;

    [GraphQLName("description")]
    [GraphQLDescription("Описание статьи")]
    public string Description { get; init; } = string.Empty;

    [GraphQLName("text")]
    [GraphQLDescription("Содержимое статьи")]
    public string Text { get; init; } = string.Empty;

    [GraphQLName("isPublished")]
    [GraphQLDescription("true")]
    public bool IsPublished { get; init; }

    [GraphQLName("isPinned")]
    [GraphQLDescription("false")]
    public bool IsPinned { get; init; }
}