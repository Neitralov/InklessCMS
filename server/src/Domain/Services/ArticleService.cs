namespace Domain.Services;

public class ArticleService(IArticleRepository articleRepository)
{
    public async Task<ErrorOr<Created>> StoreArticle(Article newArticle)
    {
        if (await articleRepository.IsArticleExists(newArticle.ArticleId))
            return Errors.Article.NonUniqueId;
        
        await articleRepository.AddArticle(newArticle);
        await articleRepository.SaveChanges();

        return Result.Created;
    }

    public async Task<ErrorOr<Article>> GetArticle(string articleId)
    {
        var article = await articleRepository.FindArticleById(articleId);

        return article is not null ? article : Errors.Article.NotFound;
    }
    
    public async Task<PagedList<Article>> GetArticles(int page, int size)
    {
        var drafts = await articleRepository.GetArticles(page, size);

        return drafts;
    }
    
    public async Task<PagedList<Article>> GetPublishedArticles(int page, int size)
    {
        var publishedArticles = await articleRepository.GetPublishedArticles(page, size);

        return publishedArticles;
    }

    public async Task<ErrorOr<Updated>> UpdateArticle(Article updatedArticle)
    {
        var currentArticle = await articleRepository.FindArticleById(updatedArticle.ArticleId);

        if (currentArticle is null)
            return Errors.Article.NotFound;
        
        var result = currentArticle.Update(updatedArticle);

        if (result == Result.Updated)
            await articleRepository.SaveChanges();

        return result;
    }

    public async Task<ErrorOr<Updated>> ChangePinState(string articleId)
    {
        var article = await articleRepository.FindArticleById(articleId);

        if (article is null)
            return Errors.Article.NotFound;

        var result = article.ChangePinState();

        if (result == Result.Updated)
            await articleRepository.SaveChanges();

        return result;
    }

    public async Task<ErrorOr<Updated>> IncreaseViewsCounter(string articleId)
    {
        var article = await articleRepository.FindArticleById(articleId);

        if (article is null)
            return Errors.Article.NotFound;

        var result = article.IncreaseViewsCounter();

        if (result == Result.Updated)
            await articleRepository.SaveChanges();

        return result.Value;
    }

    public async Task<ErrorOr<Deleted>> DeleteArticle(string articleId)
    {
        var result = await articleRepository.DeleteArticle(articleId);

        if (result)
            await articleRepository.SaveChanges();

        return result ? Result.Deleted : Errors.Article.NotFound;
    }
}
