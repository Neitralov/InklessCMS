namespace WebAPI.GraphQL.Queries;

[GraphQLName("ArticleQueries")]
public sealed class GqlArticleQueries
{
    [GraphQLName("articles")]
    [GraphQLDescription("Список всех статей")]
    [GqlAuthorize(Policy = "CanManageArticles")]
    public async Task<GqlArticle[]> GetArticlesAsync(
        [Service] IArticleRepository articleRepository,
        [Service] IHttpContextAccessor httpContextAccessor,
        PageOptions pageOptions,
        CancellationToken cancellationToken)
    {
        var articles = await articleRepository.GetArticlesAsync(pageOptions, cancellationToken);

        if (httpContextAccessor.HttpContext is not null)
            httpContextAccessor.HttpContext.Response.Headers
                .Append("X-Total-Count", articles.TotalCount.ToString());

        return articles.Select(article => article.ToGqlArticle()).ToArray();
    }

    [GraphQLName("publishedArticles")]
    [GraphQLDescription("Список опубликованных статей")]
    public async Task<GqlArticle[]> GetPublishedArticlesAsync(
        [Service] IArticleRepository articleRepository,
        [Service] IHttpContextAccessor httpContextAccessor,
        PageOptions pageOptions,
        CancellationToken cancellationToken)
    {
        var publishedArticles = await articleRepository.GetPublishedArticlesAsync(pageOptions, cancellationToken);

        if (httpContextAccessor.HttpContext is not null)
            httpContextAccessor.HttpContext.Response.Headers
                .Append("X-Total-Count", publishedArticles.TotalCount.ToString());

        // TODO: Нужно написать мапперы, как это сделано в REST сейчас
        return publishedArticles.Select(article => article.ToGqlArticle()).ToArray();
    }

    [GraphQLName("article")]
    [GraphQLDescription("Статья")]
    public async Task<GqlArticle> GetArticleAsync(
        [Service] IArticleRepository articleRepository,
        [Service] IAuthorizationService authService,
        [Service] IHttpContextAccessor httpContextAccessor,
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
