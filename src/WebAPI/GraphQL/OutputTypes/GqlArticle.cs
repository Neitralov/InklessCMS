namespace WebAPI.GraphQL.OutputTypes;

[GraphQLName("article")]
public sealed class GqlArticle(Article Article)
{
    [GraphQLName("articleId")]
    [GraphQLDescription("some-article-id")]
    public string ArticleId { get; init; } = Article.ArticleId;

    [GraphQLName("title")]
    [GraphQLDescription("Заголовок статьи")]
    public string Title { get; init; } = Article.Title;

    [GraphQLName("description")]
    [GraphQLDescription("Описание статьи")]
    public string Description { get; init; } = Article.Description;

    [GraphQLName("text")]
    [GraphQLDescription("Содержимое статьи")]
    public string Text { get; init; } = Article.Text;

    [GraphQLName("isPublished")]
    [GraphQLDescription("true")]
    public bool IsPublished { get; init; } = Article.IsPublished;

    [GraphQLName("publishDate")]
    [GraphQLDescription("2024-01-01T08:00:00Z")]
    public DateTime? PublishDate { get; init; } = Article.PublishDate;

    [GraphQLName("views")]
    [GraphQLDescription("0")]
    public int Views { get; init; } = Article.Views;

    [GraphQLName("isPinned")]
    [GraphQLDescription("false")]
    public bool IsPinned { get; init; } = Article.IsPinned;
}
