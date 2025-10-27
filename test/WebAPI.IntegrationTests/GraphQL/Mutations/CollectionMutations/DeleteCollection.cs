namespace WebAPI.IntegrationTests.GraphQL.Mutations.CollectionMutations;

public static partial class Mutations
{
    public static async Task<string> DeleteCollection(this GraphQLHttpClient gqlClient, string collectionId)
    {
        var gqlResponse = await gqlClient.SendMutationAsync(
            request: DeleteCollection(collectionId),
            defineResponseType: () => new { collectionMutations = new { deleteCollection = string.Empty } });
            
        if (gqlResponse.Errors is not null)
            throw new GraphQLException(message: gqlResponse.Errors.First().Message);
        
        return gqlResponse.Data.collectionMutations.deleteCollection;
    }
    
    private static GraphQLHttpRequest DeleteCollection(string collectionId) => new()
    {
        Query =
            $$"""
              mutation DeleteCollection {
                collectionMutations {
                  deleteCollection(collectionId: "{{collectionId}}")
                }
              }
              """
    };
}
