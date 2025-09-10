using WebAPI.IntegrationTests.GraphQL.Fragments;

namespace WebAPI.IntegrationTests.GraphQL.Queries;

public static partial class Queries
{
    public static async Task<GqlArticle> GetArticle(this GraphQLHttpClient gqlClient, string articleId)
    {
        var gqlResponse = await gqlClient.SendQueryAsync(
            request: GetArticle(articleId),
            defineResponseType: () => new { articleQueries = new { article = new GqlArticle() }});
        
        return gqlResponse.Data.articleQueries.article;
    }
    
    private static GraphQLHttpRequest GetArticle(string articleId) => new()
    {
        Query =
            $$"""
              {{ArticleFragment.Fragment}}

              query GetArticle {
                articleQueries {
                  article (articleId: "{{articleId}}") {
                    ...ArticleFields
                  }
                }
              }
              """
    };
}

