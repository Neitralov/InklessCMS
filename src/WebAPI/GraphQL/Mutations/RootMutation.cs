namespace WebAPI.GraphQL.Mutations;

[GraphQLName("RootMutation")]
public sealed class RootMutation
{
    [GraphQLName("articleMutations")]
    [GraphQLDescription("Мутации статей")]
    public GqlArticleMutations ArticleMutations { get; } = new();

    [GraphQLName("collectionMutations")]
    [GraphQLDescription("Мутации коллекций")]
    public GqlCollectionMutations CollectionMutations { get; } = new();

    [GraphQLName("userMutations")]
    [GraphQLDescription("Мутации пользователей")]
    public GqlUserMutations UserMutations { get; } = new();
}
