using WebAPI.IntegrationTests.GraphQL.Fragments;

namespace WebAPI.IntegrationTests.GraphQL.Queries;

public static partial class Queries
{
    public static async Task<IReadOnlyCollection<GqlArticle>> GetArticles(
        this GraphQLHttpClient gqlClient, 
        PageOptions pageOptions)
    {
        var gqlResponse = await gqlClient.SendQueryAsync(
            request: GetArticles(pageOptions),
            defineResponseType: () => new { articleQueries = new { articles = new List<GqlArticle>() }});
        
        return gqlResponse.Data.articleQueries.articles;
    }
    
    private static GraphQLHttpRequest GetArticles(PageOptions pageOptions) => new()
    {
        Query =
            $$"""
              {{ArticleFragment.Fragment}}

              query GetArticles {
                articleQueries {
                  articles (pageOptions: {
                   page: {{pageOptions.Page}},
                   size: {{pageOptions.Size}}
                  }) {
                    ...ArticleFields
                  }
                }
              }
              """
    };
}
