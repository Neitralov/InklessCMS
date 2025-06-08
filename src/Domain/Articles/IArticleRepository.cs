namespace Domain.Articles;

public interface IArticleRepository
{
    Task AddArticleAsync(Article newArticle);
    Task<ErrorOr<Article>> FindArticleByIdAsync(string articleId);
    Task<PagedList<Article>> GetArticlesAsync(PageOptions pageOptions, CancellationToken cancellationToken);
    Task<PagedList<Article>> GetPublishedArticlesAsync(PageOptions pageOptions, CancellationToken cancellationToken);
    Task<bool> IsArticleExistsAsync(string articleId);
    Task<ErrorOr<Deleted>> DeleteArticleAsync(string articleId);
    Task SaveChangesAsync();
}
