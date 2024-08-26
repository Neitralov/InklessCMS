namespace WebAPI.Contracts.Collection;

public record CollectionPreviewResponse(
    string CollectionId,
    string Title
)
{
    ///<example>some-collection-id</example>
    public string CollectionId { get; init; } = CollectionId;
    ///<example>Название коллекции</example>
    public string Title { get; init; } = Title;
}
