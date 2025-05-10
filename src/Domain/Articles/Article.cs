namespace Domain.Articles;

public sealed partial class Article
{
    public string ArticleId { get; private set; } = string.Empty;
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string Text { get; private set; } = string.Empty;
    public bool IsPublished { get; private set; }
    public DateTime? PublishDate { get; private set; }
    public DateTime CreationDate { get; private set; }
    public int Views { get; private set; }
    public bool IsPinned { get; private set; }

    public const int MinIdLength = 3;
    public const int MaxIdLength = 64;
    public const int MinTitleLength = 3;
    public const int MaxTitleLength = 64;
    public const int MaxDescriptionLength = 64;
    public const string ArticleIdPattern = "^[a-zA-Z0-9-]+$";

    private Article() { }

    public static ErrorOr<Article> Create(
        string articleId,
        string title,
        string description,
        string text,
        bool isPublished,
        bool isPinned = false
    )
    {
        List<Error> errors = [];

        if (articleId.Trim().Length is < MinIdLength or > MaxIdLength)
            errors.Add(Errors.InvalidIdLength);

        if (!Regex.IsMatch(articleId.Trim(), ArticleIdPattern, RegexOptions.Compiled))
            errors.Add(Errors.InvalidId);

        if (title.Trim().Length is < MinTitleLength or > MaxTitleLength)
            errors.Add(Errors.InvalidTitleLength);

        if (description.Trim().Length > MaxDescriptionLength)
            errors.Add(Errors.InvalidDescriptionLength);

        return errors.Any() ? errors : new Article
        {
            ArticleId = articleId.Trim(),
            Title = title.Trim(),
            Description = description.Trim(),
            Text = text.Trim(),
            IsPublished = isPublished,
            PublishDate = isPublished ? DateTime.UtcNow : null,
            CreationDate = DateTime.UtcNow,
            IsPinned = isPinned
        };
    }

    public ErrorOr<Updated> Update(Article updatedArticle)
    {
        Title = updatedArticle.Title;
        Description = updatedArticle.Description;
        Text = updatedArticle.Text;
        IsPinned = updatedArticle.IsPinned;

        if (IsPublished == false && updatedArticle.IsPublished)
        {
            IsPublished = updatedArticle.IsPublished;
            PublishDate = updatedArticle.PublishDate;
        }

        return Result.Updated;
    }

    public ErrorOr<Updated> IncreaseViewsCounter()
    {
        Views++;

        return Result.Updated;
    }

    public ErrorOr<Updated> ChangePinState()
    {
        IsPinned = !IsPinned;

        return Result.Updated;
    }
}
