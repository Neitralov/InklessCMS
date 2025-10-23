using WebAPI.IntegrationTests.GraphQL.Fragments;

namespace WebAPI.IntegrationTests.GraphQL.Mutations.CollectionMutations;

public static partial class Mutations
{
    public static async Task<GqlCollection> UpdateCollection(this GraphQLHttpClient gqlClient, GqlCollectionInput input)
    {
        var gqlResponse = await gqlClient.SendMutationAsync(
            request: UpdateCollection(input),
            defineResponseType: () => new { collectionMutations = new { updateCollection = new GqlCollection() }});
        
        return gqlResponse.Data.collectionMutations.updateCollection;
    }
    
    private static GraphQLHttpRequest UpdateCollection(GqlCollectionInput input) => new()
    {
        Query =
            $$"""
              {{ArticleFragment.Fragment}}

              mutation UpdateCollection($input: collectionInput!) {
                collectionMutations {
                  updateCollection(input: $input) {
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
