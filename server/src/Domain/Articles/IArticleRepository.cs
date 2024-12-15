namespace Domain.Articles;

public interface IArticleRepository
{
    Task AddArticle(Article newArticle);
    Task<ErrorOr<Article>> FindArticleById(string articleId);
    Task<PagedList<Article>> GetArticles(PageOptions pageOptions, CancellationToken cancellationToken);
    Task<PagedList<Article>> GetPublishedArticles(PageOptions pageOptions, CancellationToken cancellationToken);
    Task<bool> IsArticleExists(string articleId);
    Task<ErrorOr<Deleted>> DeleteArticle(string articleId);
    Task SaveChanges();
}