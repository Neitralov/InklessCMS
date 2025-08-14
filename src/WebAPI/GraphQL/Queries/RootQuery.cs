namespace WebAPI.GraphQL.Queries;

public sealed class RootQuery
{
    [GraphQLName("publishedArticles")]
    public async Task<GqlArticleList> GetPublishedArticlesAsync(
        [Service] IArticleRepository articleRepository,
        PageOptions pageOptions,
        CancellationToken cancellationToken)
    {
        var publishedArticles = await articleRepository.GetPublishedArticlesAsync(pageOptions, cancellationToken);

        // Через IHttpContextAccessor установить хедер с числом элементов как в контроллере

        return new GqlArticleList(
            Articles: publishedArticles.Select(article => new GqlArticle(article)),
            TotalCount: publishedArticles.TotalCount
        );
    }

    [GraphQLName("article")]
    public async Task<GqlArticle> GetArticleAsync(
        [Service] IArticleRepository articleRepository,
        [Service] IAuthorizationService authService,
        [Service] IHttpContextAccessor httpContextAccessor,
        string articleId)
    {
        var getArticleResult = await articleRepository.FindArticleByIdAsync(articleId);

        if (getArticleResult.IsError)
            throw new Exception(message: getArticleResult.Errors.First().Code);

        var authResult = await authService.AuthorizeAsync(httpContextAccessor.HttpContext!.User, "CanManageArticles");
        if (getArticleResult.Value.IsPublished == false && !authResult.Succeeded)
            throw new Exception(message: Article.Errors.NotFound.Code);

        var article = getArticleResult.Value;

        return new GqlArticle(article);
    }
}
