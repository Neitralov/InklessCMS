namespace Database.Repositories;

public sealed class ArticleRepository(DatabaseContext database) : BaseRepository(database), IArticleRepository
{
    private readonly DatabaseContext _database = database;

    public async Task AddArticle(Article newArticle) => await _database.Articles.AddAsync(newArticle);

    public async Task<ErrorOr<Article>> FindArticleById(string articleId) =>
        await _database.Articles.SingleOrDefaultAsync(article => article.ArticleId == articleId) ??
        Domain.Articles.Errors.Article.NotFound.ToErrorOr<Article>();

    public async Task<PagedList<Article>> GetArticles(
        PageOptions pageOptions,
        CancellationToken cancellationToken) =>
        await _database.Articles
            .AsNoTracking()
            .OrderByDescending(article => article.CreationDate)
            .ToPagedList(pageOptions, cancellationToken);

    public async Task<PagedList<Article>> GetPublishedArticles(
        PageOptions pageOptions,
        CancellationToken cancellationToken) =>
        await _database.Articles
            .AsNoTracking()
            .Where(article => article.IsPublished)
            .OrderByDescending(article => article.IsPinned)
            .ThenByDescending(article => article.PublishDate)
            .ToPagedList(pageOptions, cancellationToken);

    public async Task<bool> IsArticleExists(string articleId) =>
        await _database.Articles.AnyAsync(article => article.ArticleId == articleId);

    public async Task<ErrorOr<Deleted>> DeleteArticle(string articleId)
    {
        var article = await FindArticleById(articleId);

        if (article.IsError)
            return article.Errors;

        _database.Articles.Remove(article.Value);

        return Result.Deleted;
    }
}