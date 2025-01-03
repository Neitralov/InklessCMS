namespace WebAPI.IntegrationTests.ArticlesController;

public class ArticlesControllerTests 
{
    static protected IEnumerable<CreateArticleRequest> GenerateCreateArticleRequests(int count)
    {
        for (var index = 1; index <= count; index++)
        {
            yield return new CreateArticleRequest
            (
                ArticleId: $"article-{index}",
                Title: "Test title",
                Description: "Test description",
                Text: "Test text",
                IsPinned: false,
                IsPublished: false
            );   
        }
    }

    static protected CreateArticleRequest GenerateCreateArticleRequest()
    {
        return new CreateArticleRequest
        (
            ArticleId: "article-id",
            Title: "Test title",
            Description: "Test description",
            Text: "Test text",
            IsPinned: false,
            IsPublished: false
        );
    }
}