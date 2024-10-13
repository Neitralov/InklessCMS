namespace Domain.Interfaces;

public interface IArticleRepository
{
    Task AddArticle(Article newArticle);
    Task<ErrorOr<Article>> FindArticleById(string articleId);
    Task<PagedList<Article>> GetArticles(int page, int size, CancellationToken cancellationToken);
    Task<PagedList<Article>> GetPublishedArticles(int page, int size, CancellationToken cancellationToken);
    Task<bool> IsArticleExists(string articleId);
    Task<ErrorOr<Deleted>> DeleteArticle(string articleId);
    Task SaveChanges();
}
