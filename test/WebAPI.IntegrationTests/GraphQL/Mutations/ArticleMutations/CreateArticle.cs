using WebAPI.IntegrationTests.GraphQL.Fragments;

namespace WebAPI.IntegrationTests.GraphQL.Mutations.ArticleMutations;

public static partial class Mutations
{
    public static async Task<GqlArticle> CreateArticle(this GraphQLHttpClient gqlClient, GqlArticleInput input)
    {
        var gqlResponse = await gqlClient.SendMutationAsync(
            request: CreateArticle(input),
            defineResponseType: () => new { articleMutations = new { createArticle = new GqlArticle() }});
        
        return gqlResponse.Data.articleMutations.createArticle;
    }
    
    private static GraphQLHttpRequest CreateArticle(GqlArticleInput input) => new()
    {
        Query =
            $$"""
              {{ArticleFragment.Fragment}}

              mutation CreateArticle($input: articleInput!) {
                articleMutations {
                  createArticle(input: $input) {
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
