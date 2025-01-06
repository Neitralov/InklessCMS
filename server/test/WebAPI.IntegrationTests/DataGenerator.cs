namespace WebAPI.IntegrationTests;

public static class DataGenerator
{
    public static class Article
    {
        public static CreateArticleRequest GetCreateRequest()
        {
            return new CreateArticleRequest
            (
                ArticleId: "article-id",
                Title: "Title",
                Description: "Description",
                Text: "Text",
                IsPinned: false,
                IsPublished: false
            );
        }
    
        public static UpdateArticleRequest GetUpdateRequest()
        {
            return new UpdateArticleRequest
            (
                ArticleId: "article-id",
                Title: "Title",
                Description: "Description",
                Text: "Text",
                IsPinned: false,
                IsPublished: false
            );
        }
    }
}