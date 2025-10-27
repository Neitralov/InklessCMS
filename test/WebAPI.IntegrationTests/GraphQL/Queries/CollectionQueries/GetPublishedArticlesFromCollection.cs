namespace WebAPI.IntegrationTests.GraphQL.Queries.CollectionQueries;

public static partial class Queries
{
    public static async Task<IReadOnlyCollection<GqlArticle>> GetPublishedArticlesFromCollection(
        this GraphQLHttpClient gqlClient, 
        string collectionId,
        PageOptions pageOptions)
    {
        var gqlResponse = await gqlClient.SendQueryAsync(
            request: GetPublishedArticlesFromCollection(collectionId, pageOptions),
            defineResponseType: () => new
            {
                collectionQueries = new { publishedArticlesFromCollection = new List<GqlArticle>() }
            });
            
        if (gqlResponse.Errors is not null)
            throw new GraphQLException(message: gqlResponse.Errors.First().Message);
        
        return gqlResponse.Data.collectionQueries.publishedArticlesFromCollection;
    }
    
    private static GraphQLHttpRequest GetPublishedArticlesFromCollection(
        string collectionId,
        PageOptions pageOptions) => new()
    {
        Query =
            $$"""
              {{ArticleFragment.Fragment}}

              query GetPublishedArticlesFromCollection {
                collectionQueries {
                  publishedArticlesFromCollection (collectionId: "{{collectionId}}", pageOptions: {
                    page: {{pageOptions.Page}},
                    size: {{pageOptions.Size}}
                  }) {
                    ...ArticleFields
                  }
                }
              }
              """
    };
}
