using WebAPI.IntegrationTests.GraphQL.Fragments;

namespace WebAPI.IntegrationTests.GraphQL.Queries.CollectionQueries;

public static partial class Queries
{
    public static async Task<IReadOnlyCollection<GqlCollection>> GetCollections(this GraphQLHttpClient gqlClient)
    {
        var gqlResponse = await gqlClient.SendQueryAsync(
            request: GetCollections(),
            defineResponseType: () => new { collectionQueries = new { collections = new List<GqlCollection>() }});
        
        return gqlResponse.Data.collectionQueries.collections;
    }
    
    private static GraphQLHttpRequest GetCollections() => new()
    {
        Query =
            $$"""
              {{ArticleFragment.Fragment}}

              query GetCollections {
                collectionQueries {
                  collections {
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
