namespace Database.Repositories;

public class ArticleRepository(DatabaseContext database) : IArticleRepository
{
    public async Task AddArticle(Article newArticle)
    {
        await database.AddAsync(newArticle);
    }

    public async Task<ErrorOr<Article>> FindArticleById(string articleId)
    {
        var foundArticle = await database.Articles.SingleOrDefaultAsync(article => article.ArticleId == articleId);

        return foundArticle is null ? Errors.Article.NotFound : foundArticle;
    }

    public async Task<PagedList<Article>> GetArticles(int page, int size, CancellationToken cancellationToken)
    {
        return await database.Articles
            .AsNoTracking()
            .OrderByDescending(article => article.CreationDate)
            .ToPagedList(page, size, cancellationToken);
    }

    public async Task<PagedList<Article>> GetPublishedArticles(int page, int size, CancellationToken cancellationToken)
    {
        return await database.Articles
            .AsNoTracking()
            .Where(article => article.IsPublished)
            .OrderByDescending(article => article.IsPinned)
            .ThenByDescending(article => article.PublishDate)
            .ToPagedList(page, size, cancellationToken);
    }

    public async Task<bool> IsArticleExists(string articleId)
    {
        return await database.Articles.AnyAsync(article => article.ArticleId == articleId);
    }

    public async Task<ErrorOr<Deleted>> DeleteArticle(string articleId)
    {
        var storedArticle = await FindArticleById(articleId);

        if (storedArticle.IsError)
            return storedArticle.Errors;

        database.Remove(storedArticle);

        return Result.Deleted;
    }

    public async Task SaveChanges()
    {
        await database.SaveChangesAsync();
    }
}
