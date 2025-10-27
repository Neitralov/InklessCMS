namespace Domain.Collections;

public sealed partial record Collection
{
    public string CollectionId { get; private set; } = string.Empty;
    public string Title { get; private set; } = string.Empty;
    public IReadOnlyList<Article> Articles => _articles;
    private readonly List<Article> _articles = [];

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
            errors.Add(Errors.InvalidIdLength);

        if (!Regex.IsMatch(collectionId.Trim(), CollectionIdPattern, RegexOptions.Compiled))
            errors.Add(Errors.InvalidId);

        if (title.Trim().Length is < MinTitleLength or > MaxTitleLength)
            errors.Add(Errors.InvalidTitleLength);

        return errors.Any() ? errors : new Collection
        {
            CollectionId = collectionId.Trim(),
            Title = title.Trim()
        };
    }

    public void Update(Collection updatedCollection) => Title = updatedCollection.Title;

    public void AddArticle(Article articleToAdd) => _articles.Add(articleToAdd);

    public ErrorOr<Deleted> DeleteArticle(string idOfArticleToDelete)
    {
        var articleToRemove = _articles.SingleOrDefault(article => article.ArticleId == idOfArticleToDelete);

        if (articleToRemove is null)
            return Errors.ArticleNotFound;

        _articles.Remove(articleToRemove);

        return Result.Deleted;
    }

    public ErrorOr<Article> FindArticleById(string articleIdToFind) =>
        _articles.SingleOrDefault(article => article.ArticleId == articleIdToFind)
            ?? Errors.ArticleNotFound.ToErrorOr<Article>();
}
