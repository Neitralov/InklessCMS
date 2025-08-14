namespace WebAPI.GraphQL.Mutations;

public sealed class RootMutation
{
    [HotChocolate.Authorization.Authorize(Policy = "CanManageArticles")]
    public async Task<GqlArticle> CreateArticleAsync(
        [Service] IArticleRepository articleRepository,
        CreateArticleRequest request)
    {
        var requestToArticleResult = CreateArticleFrom(request);

        if (requestToArticleResult.IsError)
            throw new Exception(message: requestToArticleResult.Errors.First().Code);

        var article = requestToArticleResult.Value;

        if (await articleRepository.IsArticleExistsAsync(article.ArticleId))
            throw new Exception(message: Article.Errors.NonUniqueId.Code);

        await articleRepository.AddArticleAsync(article);
        await articleRepository.SaveChangesAsync();

        return new GqlArticle(article);
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
}
