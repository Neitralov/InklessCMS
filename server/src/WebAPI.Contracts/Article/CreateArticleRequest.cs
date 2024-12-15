namespace WebAPI.Contracts.Article;

public sealed record CreateArticleRequest(
    string ArticleId,
    string Title,
    string Description,
    string Text,
    bool IsPublished,
    bool IsPinned
)
{
    ///<example>some-article-id</example>
    public string ArticleId { get; init; } = ArticleId;
    ///<example>Заголовок статьи</example>
    public string Title { get; init; } = Title;
    ///<example>Описание статьи</example>
    public string Description { get; init; } = Description;
    ///<example>Содержимое статьи</example>
    public string Text { get; init; } = Text;
    ///<example>true</example>
    public bool IsPublished { get; init; } = IsPublished;
    ///<example>false</example>
    public bool IsPinned { get; init; } = IsPinned;
}