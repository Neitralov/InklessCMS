using WebAPI.IntegrationTests.GraphQL.Fragments;

namespace WebAPI.IntegrationTests.GraphQL.Queries.ArticleQueries;

public static partial class Queries
{
    public static async Task<IReadOnlyCollection<GqlArticle>> GetPublishedArticles(
        this GraphQLHttpClient gqlClient, 
        PageOptions pageOptions)
    {
        var gqlResponse = await gqlClient.SendQueryAsync(
            request: GetPublishedArticles(pageOptions),
            defineResponseType: () => new { articleQueries = new { publishedArticles = new List<GqlArticle>() }});
            
        if (gqlResponse.Errors is not null)
            throw new GraphQLException(message: gqlResponse.Errors.First().Message);
        
        return gqlResponse.Data.articleQueries.publishedArticles;
    }
    
    private static GraphQLHttpRequest GetPublishedArticles(PageOptions pageOptions) => new()
    {
        Query =
            $$"""
              {{ArticleFragment.Fragment}}

              query GetArticles {
                articleQueries {
                  publishedArticles (pageOptions: {
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
