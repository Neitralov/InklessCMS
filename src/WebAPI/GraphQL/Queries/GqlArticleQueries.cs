namespace WebAPI.GraphQL.Queries;

[GraphQLName("ArticleQueries")]
public sealed class GqlArticleQueries
{
    [GraphQLName("articles")]
    [GraphQLDescription("Список всех статей")]
    [GqlAuthorize(Policy = "CanManageArticles")]
    public async Task<GqlArticle[]> GetArticlesAsync(
        IArticleRepository articleRepository,
        IHttpContextAccessor httpContextAccessor,
        PageOptions pageOptions,
        CancellationToken cancellationToken)
    {
        var articles = await articleRepository.GetArticlesAsync(pageOptions, cancellationToken);

        httpContextAccessor.HttpContext?.Response.Headers
            .Append("X-Total-Count", articles.TotalCount.ToString());

        return articles.ToGqlArticles();
    }

    [GraphQLName("publishedArticles")]
    [GraphQLDescription("Список опубликованных статей")]
    public async Task<GqlArticle[]> GetPublishedArticlesAsync(
        IArticleRepository articleRepository,
        IHttpContextAccessor httpContextAccessor,
        PageOptions pageOptions,
        CancellationToken cancellationToken)
    {
        var publishedArticles = await articleRepository.GetPublishedArticlesAsync(pageOptions, cancellationToken);

        httpContextAccessor.HttpContext?.Response.Headers
            .Append("X-Total-Count", publishedArticles.TotalCount.ToString());

        return publishedArticles.ToGqlArticles();
    }

    [GraphQLName("article")]
    [GraphQLDescription("Статья")]
    public async Task<GqlArticle> GetArticleAsync(
        IArticleRepository articleRepository,
        IAuthorizationService authService,
        IHttpContextAccessor httpContextAccessor,
        string articleId)
    {
        var getArticleResult = await articleRepository.FindArticleByIdAsync(articleId);

        if (getArticleResult.IsError)
            throw new Exception(getArticleResult.FirstError.Code);

        var authResult = await authService.AuthorizeAsync(httpContextAccessor.HttpContext!.User, "CanManageArticles");
        if (getArticleResult.Value.IsPublished == false && !authResult.Succeeded)
            throw new Exception(Article.Errors.NotFound.Code);

        var article = getArticleResult.Value;

        return article.ToGqlArticle();
    }
}
