namespace WebAPI.GraphQL.OutputTypes;

[GraphQLName("article")]
public sealed class GqlArticle
{
    [GraphQLName("articleId")]
    [GraphQLDescription("some-article-id")]
    public string ArticleId { get; init; }

    [GraphQLName("title")]
    [GraphQLDescription("Заголовок статьи")]
    public string Title { get; init; }

    [GraphQLName("description")]
    [GraphQLDescription("Описание статьи")]
    public string Description { get; init; }

    [GraphQLName("text")]
    [GraphQLDescription("Содержимое статьи")]
    public string Text { get; init; }

    [GraphQLName("isPublished")]
    [GraphQLDescription("true")]
    public bool IsPublished { get; init; }

    [GraphQLName("publishDate")]
    [GraphQLDescription("2024-01-01T08:00:00Z")]
    public DateTime? PublishDate { get; init; }

    [GraphQLName("views")]
    [GraphQLDescription("0")]
    public int Views { get; init; }

    [GraphQLName("isPinned")]
    [GraphQLDescription("false")]
    public bool IsPinned { get; init; }
}

public static class GqlArticleExtensions
{
    public static GqlArticle ToGqlArticle(this Article article) => new()
    {
        ArticleId = article.ArticleId,
        Title = article.Title,
        Description = article.Description,
        Text = article.Text,
        IsPublished = article.IsPublished,
        PublishDate = article.PublishDate,
        Views = article.Views,
        IsPinned = article.IsPinned
    };
}
