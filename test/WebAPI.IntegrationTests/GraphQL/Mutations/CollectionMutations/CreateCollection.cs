using WebAPI.IntegrationTests.GraphQL.Fragments;

namespace WebAPI.IntegrationTests.GraphQL.Mutations.CollectionMutations;

public static partial class Mutations
{
    public static async Task<GqlCollection> CreateCollection(this GraphQLHttpClient gqlClient, GqlCollectionInput input)
    {
        var gqlResponse = await gqlClient.SendMutationAsync(
            request: CreateCollection(input),
            defineResponseType: () => new { collectionMutations = new { createCollection = new GqlCollection() }});
        
        return gqlResponse.Data.collectionMutations.createCollection;
    }
    
    private static GraphQLHttpRequest CreateCollection(GqlCollectionInput input) => new()
    {
        Query =
            $$"""
              {{ArticleFragment.Fragment}}

              mutation CreateCollection($input: collectionInput!) {
                collectionMutations {
                  createCollection(input: $input) {
                    collectionId
                    title
                    articles {
                      ...ArticleFields
                    }
                  }
                }
              }
              """,
        Variables = new
        {
            input
        }
    };
}
