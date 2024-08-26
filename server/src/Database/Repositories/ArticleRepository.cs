namespace Database.Repositories;

public class ArticleRepository(DatabaseContext database) : IArticleRepository
{
    public async Task AddArticle(Article newArticle)
    {
        await database.AddAsync(newArticle);
    }

    public async Task<Article?> FindArticleById(string articleId)
    {
        return await database.Articles.SingleOrDefaultAsync(article => article.ArticleId == articleId);
    }

    public async Task<PagedList<Article>> GetArticles(int page, int size)
    {
        return await database.Articles
            .AsNoTracking()
            .OrderByDescending(article => article.CreationDate)
            .ToPagedList(page, size);
    }

    public async Task<PagedList<Article>> GetPublishedArticles(string? collectionId, int page, int size)
    {
        if (collectionId is null)
        {
            return await database.Articles
                .AsNoTracking()
                .Where(article => article.IsPublished)
                .OrderByDescending(article => article.IsPinned)
                .ThenByDescending(article => article.PublishDate)
                .ToPagedList(page, size);
        }
        else
        {
            return await database.Collections
                .AsNoTracking()
                .Include(collection => collection.Articles)
                .Where(collection => collection.CollectionId == collectionId)
                .SelectMany(collection => collection.Articles)
                .Where(article => article.IsPublished)
                .OrderByDescending(article => article.IsPinned)
                .ThenByDescending(article => article.PublishDate)
                .ToPagedList(page, size);
        }
    }

    public async Task<bool> IsArticleExists(string articleId)
    {
        return await database.Articles.AnyAsync(article => article.ArticleId == articleId);
    }

    public async Task<bool> DeleteArticle(string articleId)
    {
        var storedArticle = await database.Articles.SingleOrDefaultAsync(article => article.ArticleId == articleId);

        if (storedArticle is not null)
            database.Remove(storedArticle);

        return storedArticle is not null;
    }

    public async Task SaveChanges()
    {
        await database.SaveChangesAsync();
    }
}
