using WebAPI.Contracts.Collection;

namespace WebAPI.IntegrationTests;

public static class DataGenerator
{
    public static class Article
    {
        public static CreateArticleRequest GetCreateRequest() => new
        (
            ArticleId: "article-id",
            Title: "Title",
            Description: "Description",
            Text: "Text",
            IsPinned: false,
            IsPublished: false
        );
    
        public static UpdateArticleRequest GetUpdateRequest() => new
        (
            ArticleId: "article-id",
            Title: "Title",
            Description: "Description",
            Text: "Text",
            IsPinned: false,
            IsPublished: false
        );
    }

    public static class Collection
    {
        public static CreateCollectionRequest GetCreateCollectionRequest() => new
        (
            CollectionId: "collection-id",
            Title: "Title"
        );
        
        public static UpdateCollectionRequest GetUpdateCollectionRequest() => new
        (
            CollectionId: "collection-id",
            Title: "Title"
        );
        
        public static AddArticleToCollectionRequest GetAddArticleToCollectionRequest() => new
        (
            ArticleId: "article-id"
        );
    }
}