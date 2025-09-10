using WebAPI.IntegrationTests.GraphQL.Fragments;

namespace WebAPI.IntegrationTests.GraphQL.Mutations;

public static partial class Mutations
{
    public static async Task<GqlArticle> IncreaseViews(this GraphQLHttpClient gqlClient, string articleId)
    {
        var gqlResponse = await gqlClient.SendMutationAsync(
            request: IncreaseViews(articleId),
            defineResponseType: () => new { articleMutations = new { increaseViewsCounter = new GqlArticle() } });

        return gqlResponse.Data.articleMutations.increaseViewsCounter;
    }

    private static GraphQLHttpRequest IncreaseViews(string articleId) => new()
    {
        Query =
            $$"""
              {{ArticleFragment.Fragment}}

              mutation ChangePinState {
                articleMutations {
                  increaseViewsCounter (articleId: "{{articleId}}") {
                    ...ArticleFields
                  }
                }
              }
              """
    };
}