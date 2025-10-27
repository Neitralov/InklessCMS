namespace WebAPI.IntegrationTests.GraphQL.Mutations.CollectionMutations;

public static partial class Mutations
{
    public static async Task<string> DeleteArticleFromCollection(this GraphQLHttpClient gqlClient, string collectionId, string articleId)
    {
        var gqlResponse = await gqlClient.SendMutationAsync(
            request: DeleteArticleFromCollection(collectionId, articleId),
            defineResponseType: () => new { collectionMutations = new { deleteArticleFromCollection = string.Empty } });
            
        if (gqlResponse.Errors is not null)
            throw new GraphQLException(message: gqlResponse.Errors.First().Message);
        
        return gqlResponse.Data.collectionMutations.deleteArticleFromCollection;
    }
    
    private static GraphQLHttpRequest DeleteArticleFromCollection(string collectionId, string articleId) => new()
    {
        Query =
            $$"""
              mutation DeleteArticleFromCollection {
                collectionMutations {
                  deleteArticleFromCollection(collectionId: "{{collectionId}}", articleId: "{{articleId}}")
                }
              }
              """
    };
}
