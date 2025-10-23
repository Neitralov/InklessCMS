namespace WebAPI.GraphQL.InputTypes;

[GraphQLName("collectionInput")]
public sealed record GqlCollectionInput
{
    [GraphQLName("collectionId")]
    [GraphQLDescription("some-collection-id")]
    public string CollectionId { get; init; } = string.Empty;

    [GraphQLName("title")]
    [GraphQLDescription("Заголовок коллекции")]
    public string Title { get; init; } = string.Empty;
}
