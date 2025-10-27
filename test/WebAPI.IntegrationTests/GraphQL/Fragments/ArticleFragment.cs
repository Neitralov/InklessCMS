namespace WebAPI.IntegrationTests.GraphQL.Fragments;

public static class ArticleFragment
{
    public const string Fragment =
        """
        fragment ArticleFields on article {
          articleId
          title
          description
          text
          isPublished
          publishDate
          views
          isPinned
        }
        """;
}