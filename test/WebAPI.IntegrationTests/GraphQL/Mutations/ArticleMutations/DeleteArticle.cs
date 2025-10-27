namespace WebAPI.IntegrationTests.GraphQL.Mutations.ArticleMutations;

public static partial class Mutations
{
    public static async Task<string> DeleteArticle(this GraphQLHttpClient gqlClient, string articleId)
    {
        var gqlResponse = await gqlClient.SendMutationAsync(
            request: DeleteArticle(articleId),
            defineResponseType: () => new { articleMutations = new { deleteArticle = string.Empty } });
            
        if (gqlResponse.Errors is not null)
            throw new GraphQLException(message: gqlResponse.Errors.First().Message);
        
        return gqlResponse.Data.articleMutations.deleteArticle;
    }
    
    private static GraphQLHttpRequest DeleteArticle(string articleId) => new()
    {
        Query =
            $$"""
              mutation DeleteArticle {
                articleMutations {
                  deleteArticle (articleId: "{{articleId}}")
                }
              }
              """
    };
}
