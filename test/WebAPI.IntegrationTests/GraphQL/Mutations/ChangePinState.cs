using WebAPI.IntegrationTests.GraphQL.Fragments;

namespace WebAPI.IntegrationTests.GraphQL.Mutations;

public static partial class Mutations
{
    public static async Task<GqlArticle> ChangePinState(this GraphQLHttpClient gqlClient, string articleId)
    {
        var gqlResponse = await gqlClient.SendMutationAsync(
            request: ChangePinState(articleId),
            defineResponseType: () => new { articleMutations = new { changePinState = new GqlArticle() } });

        return gqlResponse.Data.articleMutations.changePinState;
    }

    private static GraphQLHttpRequest ChangePinState(string articleId) => new()
    {
        Query =
            $$"""
              {{ArticleFragment.Fragment}}

              mutation ChangePinState {
                articleMutations {
                  changePinState (articleId: "{{articleId}}") {
                    ...ArticleFields
                  }
                }
              }
              """
    };
}

