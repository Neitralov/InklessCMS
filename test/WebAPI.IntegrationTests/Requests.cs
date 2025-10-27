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
    }

    public static class User
    {
        public static GqlLoginInput LoginInput { get; } = new()
        {
            Email = "admin@example.ru",
            Password = "admin"
        };

        public static GqlRefreshTokenInput RefreshTokenInput { get; } = new()
        {
            ExpiredAccessToken = "placeYourAccessToken",
            RefreshToken = "placeYourRefreshToken"
        };
    }
}
