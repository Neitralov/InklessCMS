namespace WebAPI.Contracts.Collection;

public record AddArticleToCollectionRequest(string ArticleId)
{
    ///<example>some-article-id</example>
    public string ArticleId { get; init; } = ArticleId;
}
