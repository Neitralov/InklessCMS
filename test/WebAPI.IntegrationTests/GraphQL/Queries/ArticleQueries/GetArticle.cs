namespace WebAPI.IntegrationTests.GraphQL.Queries.ArticleQueries;

public static partial class Queries
{
    public static async Task<GqlArticle> GetArticle(this GraphQLHttpClient gqlClient, string articleId)
    {
        var gqlResponse = await gqlClient.SendQueryAsync(
            request: GetArticle(articleId),
            defineResponseType: () => new { articleQueries = new { article = new GqlArticle() } });
            
        if (gqlResponse.Errors is not null)
            throw new GraphQLException(message: gqlResponse.Errors.First().Message);
        
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

