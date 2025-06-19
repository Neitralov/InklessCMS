public static class ArticleMapper
{
    public static ArticleResponse MapToArticleResponse(this Article article)
    {
        return new ArticleResponse(
            article.ArticleId,
            article.Title,
            article.Description,
            article.Text,
            article.IsPublished,
            article.PublishDate,
            article.Views,
            article.IsPinned);
    }

    public static ArticlePreviewResponse MapToArticlePreviewResponse(this Article article)
    {
        return new ArticlePreviewResponse(
            article.ArticleId,
            article.Title,
            article.Description,
            article.IsPublished,
            article.PublishDate,
            article.Views,
            article.IsPinned);
    }

    public static List<ArticlePreviewResponse> MapToArticlePreviewResponseList(this List<Article> articles)
    {
        return articles
            .Select(article => article.MapToArticlePreviewResponse())
            .ToList();
    }
}
