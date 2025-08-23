namespace WebAPI.GraphQL.Mutations;

[GraphQLName("ArticleMutations")]
public sealed class GqlArticleMutations
{
    [GraphQLName("createArticle")]
    [GraphQLDescription("Создать статью")]
    [GqlAuthorize(Policy = "CanManageArticles")]
    public async Task<GqlArticle> CreateArticleAsync(
        [Service] IArticleRepository articleRepository,
        CreateArticleRequest request)
    {
        var requestToArticleResult = CreateArticleFrom(request);

        if (requestToArticleResult.IsError)
            throw new Exception(requestToArticleResult.FirstError.Code);

        var article = requestToArticleResult.Value;

        if (await articleRepository.IsArticleExistsAsync(article.ArticleId))
            throw new Exception(Article.Errors.NonUniqueId.Code);

        await articleRepository.AddArticleAsync(article);
        await articleRepository.SaveChangesAsync();

        return new GqlArticle(article);
    }

    [GraphQLName("updateArticle")]
    [GraphQLDescription("Обновить статью")]
    [GqlAuthorize(Policy = "CanManageArticles")]
    public async Task<GqlArticle> UpdateArticleAsync(
        [Service] IArticleRepository articleRepository,
        UpdateArticleRequest request)
    {
        var requestToArticleResult = CreateArticleFrom(request);

        if (requestToArticleResult.IsError)
            throw new Exception(requestToArticleResult.FirstError.Code);

        var updatedArticle = requestToArticleResult.Value;

        var article = await articleRepository.FindArticleByIdAsync(updatedArticle.ArticleId);

        if (article.IsError)
            throw new Exception(article.FirstError.Code);

        article.Value.Update(updatedArticle);
        await articleRepository.SaveChangesAsync();

        return new GqlArticle(article.Value);
    }

    [GraphQLName("changePinState")]
    [GraphQLDescription("Закрепить/открепить статью")]
    [GqlAuthorize(Policy = "CanManageArticles")]
    public async Task<GqlArticle> ChangePinStateAsync(
        [Service] IArticleRepository articleRepository,
        string articleId)
    {
        var article = await articleRepository.FindArticleByIdAsync(articleId);

        if (article.IsError)
            throw new Exception(article.FirstError.Code);

        article.Value.ChangePinState();
        await articleRepository.SaveChangesAsync();

        return new GqlArticle(article.Value);
    }

    [GraphQLName("increaseViewsCounter")]
    [GraphQLDescription("Увеличить счетчик просмотров статьи на единицу")]
    public async Task<GqlArticle> IncreaseViewsCounterAsync(
        [Service] IArticleRepository articleRepository,
        string articleId)
    {
        var article = await articleRepository.FindArticleByIdAsync(articleId);

        if (article.IsError)
            throw new Exception(article.FirstError.Code);

        article.Value.IncreaseViewsCounter();
        await articleRepository.SaveChangesAsync();

        return new GqlArticle(article.Value);
    }

    [GraphQLName("deleteArticle")]
    [GraphQLDescription("Удалить статью")]
    [GqlAuthorize(Policy = "CanManageArticles")]
    public async Task<string> DeleteArticleAsync(
        [Service] IArticleRepository articleRepository,
        string articleId)
    {
        var result = await articleRepository.DeleteArticleAsync(articleId);

        if (result.IsError)
            throw new Exception(result.FirstError.Code);

        await articleRepository.SaveChangesAsync();

        return articleId;
    }

    private static ErrorOr<Article> CreateArticleFrom(CreateArticleRequest request)
    {
        return Article.Create(
            request.ArticleId,
            request.Title,
            request.Description,
            request.Text,
            request.IsPublished,
            request.IsPinned);
    }

    private static ErrorOr<Article> CreateArticleFrom(UpdateArticleRequest request)
    {
        return Article.Create(
            request.ArticleId,
            request.Title,
            request.Description,
            request.Text,
            request.IsPublished,
            request.IsPinned);
    }
}
