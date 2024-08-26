namespace Domain.Entities;

public class Collection
{
    public string CollectionId { get; private set; } = string.Empty;
    public string Title { get; private set; } = string.Empty;
    public IReadOnlyList<Article> Articles => _articles;
    private List<Article> _articles = [];

    public const int MinIdLength = 3;
    public const int MaxIdLength = 32;
    public const int MinTitleLength = 3;
    public const int MaxTitleLength = 32;
    public const string CollectionIdPattern = "^[a-zA-Z0-9-]+$";

    private Collection() { }

    public static ErrorOr<Collection> Create(string collectionId, string title)
    {
        List<Error> errors = [];

        if (collectionId.Trim().Length is < MinIdLength or > MaxIdLength)
            errors.Add(Errors.Collection.InvalidIdLength);

        if (!Regex.IsMatch(collectionId.Trim(), CollectionIdPattern, RegexOptions.Compiled))
            errors.Add(Errors.Collection.InvalidId);

        if (title.Trim().Length is < MinTitleLength or > MaxTitleLength)
            errors.Add(Errors.Collection.InvalidTitleLength);

        if (errors.Count > 0)
            return errors;

        return new Collection
        {
            CollectionId = collectionId.Trim(),
            Title = title.Trim()
        };
    }

    public ErrorOr<Updated> Update(Collection updatedCollection)
    {
        Title = updatedCollection.Title;

        return Result.Updated;
    }

    public ErrorOr<Success> AddArticle(Article articleToAdd)
    {
        _articles.Add(articleToAdd);

        return Result.Success;
    }

    public ErrorOr<Deleted> DeleteArticle(string idOfArticleToDelete)
    {
        var articleToRemove = _articles.SingleOrDefault(article => article.ArticleId == idOfArticleToDelete);

        if (articleToRemove is null)
            return Errors.Collection.ArticleNotFound;

        _articles.Remove(articleToRemove);

        return Result.Deleted;
    }
}
