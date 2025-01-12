using WebAPI.Contracts.Article;

namespace WebAPI.Contracts.Collection;

public sealed record CollectionResponse(
    string CollectionId,
    string Title,
    List<ArticlePreviewResponse> Articles)
{
    ///<example>some-collection-id</example>
    public string CollectionId { get; init; } = CollectionId;
    ///<example>Название коллекции</example>
    public string Title { get; init; } = Title;
}