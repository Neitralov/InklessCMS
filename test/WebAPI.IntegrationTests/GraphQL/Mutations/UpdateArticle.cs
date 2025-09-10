using WebAPI.IntegrationTests.GraphQL.Fragments;

namespace WebAPI.IntegrationTests.GraphQL.Mutations;

public static partial class Mutations
{
    public static async Task<GqlArticle> UpdateArticle(this GraphQLHttpClient gqlClient, GqlArticleInput input)
    {
        var gqlResponse = await gqlClient.SendMutationAsync(
            request: UpdateArticle(input),
            defineResponseType: () => new { articleMutations = new { updateArticle = new GqlArticle() }});
        
        return gqlResponse.Data.articleMutations.updateArticle;
    }
    
    private static GraphQLHttpRequest UpdateArticle(GqlArticleInput input) => new()
    {
        Query =
            $$"""
              {{ArticleFragment.Fragment}}

              mutation CreateArticle($input: articleInput!) {
                articleMutations {
                  updateArticle(input: $input) {
                    ...ArticleFields
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