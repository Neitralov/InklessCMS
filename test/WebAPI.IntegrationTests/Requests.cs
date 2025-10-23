namespace WebAPI.IntegrationTests;

public static class Requests
{
    public static class Article
    {
        public static GqlArticleInput ArticleInput { get; } = new()
        {
            ArticleId =  "article-id",
            Title = "Title",
            Description = "Description",
            Text = "Text",
            IsPinned = false,
            IsPublished = false,
        };
    }

    public static class Collection
    {
        public static GqlCollectionInput CollectionInput { get; } = new()
        {
            CollectionId = "collection-id",
            Title = "Title"
        };
        
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

    public static class User
    {
        public static LoginUserRequest GetLoginUserRequest() => new
        (
            Email: "admin@example.ru",
            Password: "admin"
        );

        public static RefreshUserTokensRequest GetRefreshTokenRequest() => new
        (
            RefreshToken: "refresh-token",
            ExpiredAccessToken: "expired-access-token"
        );
    }
}
