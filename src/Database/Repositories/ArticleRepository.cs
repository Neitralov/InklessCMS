namespace Database.Repositories;

public sealed class ArticleRepository(DatabaseContext database) : BaseRepository(database), IArticleRepository
{
    private readonly DbSet<Article> _articles = database.Set<Article>();

    public async Task AddArticleAsync(Article newArticle) => await _articles.AddAsync(newArticle);

    public async Task<ErrorOr<Article>> FindArticleByIdAsync(string articleId)
    {
        return await _articles.SingleOrDefaultAsync(article => article.ArticleId == articleId) ??
            Article.Errors.NotFound.ToErrorOr<Article>();
    }

    public async Task<PagedList<Article>> GetArticlesAsync(
        PageOptions pageOptions,
        CancellationToken cancellationToken)
    {
        return await _articles
            .AsNoTracking()
            .OrderByDescending(article => article.CreationDate)
            .ToPagedList(pageOptions, cancellationToken);
    }

    public async Task<PagedList<Article>> GetPublishedArticlesAsync(
        PageOptions pageOptions,
        CancellationToken cancellationToken)
    {
        return await _articles
            .AsNoTracking()
            .Where(article => article.IsPublished)
            .OrderByDescending(article => article.IsPinned)
            .ThenByDescending(article => article.PublishDate)
            .ToPagedList(pageOptions, cancellationToken);
    }

    public async Task<bool> IsArticleExistsAsync(string articleId)
    {
        return await _articles.AnyAsync(article => article.ArticleId == articleId);
    }

    public async Task<ErrorOr<Deleted>> DeleteArticleAsync(string articleId)
    {
        var article = await FindArticleByIdAsync(articleId);

        if (article.IsError)
            return article.Errors;

        _articles.Remove(article.Value);

        return Result.Deleted;
    }
}
