using WebAPI.Contracts.Article;

namespace WebAPI.Contracts.Collection;

public record CollectionResponse(
    string CollectionId,
    string Title,
    List<ArticlePreviewResponse> Articles
)
{
    ///<example>some-collection-id</example>
    public string CollectionId { get; init; } = CollectionId;
    ///<example>Название коллекции</example>
    public string Title { get; init; } = Title;
    public List<ArticlePreviewResponse> Articles { get; init; } = Articles;
}
