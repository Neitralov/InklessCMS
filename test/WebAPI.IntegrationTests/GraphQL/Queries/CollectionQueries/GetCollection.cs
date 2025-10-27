namespace WebAPI.IntegrationTests.GraphQL.Queries.CollectionQueries;

public static partial class Queries
{
    public static async Task<GqlCollection> GetCollection(this GraphQLHttpClient gqlClient, string collectionId)
    {
        var gqlResponse = await gqlClient.SendQueryAsync(
            request: GetCollection(collectionId),
            defineResponseType: () => new { collectionQueries = new { collection = new GqlCollection() }});

        if (gqlResponse.Errors is not null)
            throw new GraphQLException(message: gqlResponse.Errors.First().Message);
        
        return gqlResponse.Data.collectionQueries.collection;
    }
    
    private static GraphQLHttpRequest GetCollection(string collectionId) => new()
    {
        Query =
            $$"""
              {{ArticleFragment.Fragment}}

              query GetCollections {
                collectionQueries {
                  collection(collectionId : "{{collectionId}}") {
                    collectionId
                    title
                    articles {
                      ...ArticleFields
                    }
                  }
                }
              }
              """
    };
}
