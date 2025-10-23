using WebAPI.IntegrationTests.GraphQL.Fragments;

namespace WebAPI.IntegrationTests.GraphQL.Mutations.CollectionMutations;

public static partial class Mutations
{
    public static async Task<GqlCollection> AddArticleToCollection(this GraphQLHttpClient gqlClient, string collectionId, string articleId)
    {
        var gqlResponse = await gqlClient.SendMutationAsync(
            request: AddArticleToCollection(collectionId, articleId),
            defineResponseType: () => new { collectionMutations = new { addArticleToCollection = new GqlCollection() } });

        return gqlResponse.Data.collectionMutations.addArticleToCollection;
    }
    
    private static GraphQLHttpRequest AddArticleToCollection(string collectionId, string articleId) => new()
    {
        Query =
            $$"""
              {{ArticleFragment.Fragment}}

              mutation AddArticleToCollection {
                collectionMutations {
                  addArticleToCollection(collectionId: "{{collectionId}}", articleId: "{{articleId}}") {
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
