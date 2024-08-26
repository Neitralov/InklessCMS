namespace Domain.Interfaces;

public interface IArticleRepository
{
    Task AddArticle(Article newArticle);
    Task<Article?> FindArticleById(string articleId);
    Task<PagedList<Article>> GetArticles(int page, int size);
    Task<PagedList<Article>> GetPublishedArticles(string? collectionId, int page, int size);
    Task<bool> IsArticleExists(string articleId);
    Task<bool> DeleteArticle(string articleId);
    Task SaveChanges();
}
