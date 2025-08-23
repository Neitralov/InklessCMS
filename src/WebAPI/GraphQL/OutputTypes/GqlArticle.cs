namespace WebAPI.GraphQL.OutputTypes;

[GraphQLName("article")]
public sealed class GqlArticle(Article article)
{
    [GraphQLName("articleId")]
    [GraphQLDescription("some-article-id")]
    public string ArticleId { get; init; } = article.ArticleId;

    [GraphQLName("title")]
    [GraphQLDescription("Заголовок статьи")]
    public string Title { get; init; } = article.Title;

    [GraphQLName("description")]
    [GraphQLDescription("Описание статьи")]
    public string Description { get; init; } = article.Description;

    [GraphQLName("text")]
    [GraphQLDescription("Содержимое статьи")]
    public string Text { get; init; } = article.Text;

    [GraphQLName("isPublished")]
    [GraphQLDescription("true")]
    public bool IsPublished { get; init; } = article.IsPublished;

    [GraphQLName("publishDate")]
    [GraphQLDescription("2024-01-01T08:00:00Z")]
    public DateTime? PublishDate { get; init; } = article.PublishDate;

    [GraphQLName("views")]
    [GraphQLDescription("0")]
    public int Views { get; init; } = article.Views;

    [GraphQLName("isPinned")]
    [GraphQLDescription("false")]
    public bool IsPinned { get; init; } = article.IsPinned;
}
