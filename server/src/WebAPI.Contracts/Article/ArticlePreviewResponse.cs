namespace WebAPI.Contracts.Article;

public sealed record ArticlePreviewResponse(
    string ArticleId,
    string Title,
    string Description,
    bool IsPublished,
    DateTime? PublishDate,
    int Views,
    bool IsPinned)
{
    ///<example>some-article-id</example>
    public string ArticleId { get; init; } = ArticleId;
    ///<example>Заголовок статьи</example>
    public string Title { get; init; } = Title;
    ///<example>Описание статьи</example>
    public string Description { get; init; } = Description;
    ///<example>true</example>
    public bool IsPublished { get; init; } = IsPublished;
    ///<example>2024-01-01T08:00:00Z</example>
    public DateTime? PublishDate { get; init; } = PublishDate;
    ///<example>0</example>
    public int Views { get; init; } = Views;
    ///<example>false</example>
    public bool IsPinned { get; init; } = IsPinned;
}
