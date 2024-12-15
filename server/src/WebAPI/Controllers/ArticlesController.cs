namespace WebAPI.Controllers;

/// <inheritdoc />
[Route("/api/articles")]
public sealed class ArticlesController(ArticleService articleService, IAuthorizationService authService) : ApiController
{
    /// <summary>Создать статью</summary>
    /// <response code="201">Статья создана</response>
    /// <response code="400">
    /// Id статьи указан некорректно;
    /// Заголовок статьи указан некорректно;
    /// Описание статьи указано некорректно
    /// </response>
    [HttpPost, Authorize(Policy = "CanManageArticles")]
    [ProducesResponseType(typeof(ArticleResponse), 201)]
    public async Task<IActionResult> CreateArticle([Required] CreateArticleRequest request)
    {
        var requestToArticleResult = CreateArticleFrom(request);

        if (requestToArticleResult.IsError)
            return Problem(requestToArticleResult.Errors);

        var article = requestToArticleResult.Value;
        var createArticleResult = await articleService.AddArticle(article);

        return createArticleResult.Match(_ => CreatedAtGetArticle(article), Problem);
    }

    /// <summary>Получить список статей</summary>
    /// <response code="200">Список статей</response>
    [HttpGet, Authorize(Policy = "CanManageArticles")]
    [ProducesResponseType(typeof(List<ArticlePreviewResponse>), 200)]
    public async Task<IActionResult> GetArticles(
        [FromQuery] PageOptions pageOptions,
        CancellationToken cancellationToken)
    {
        var articles = await articleService.GetArticles(pageOptions, cancellationToken);

        Response.Headers.Append("X-Total-Count", articles.TotalCount.ToString());
        return Ok(articles.Adapt<List<ArticlePreviewResponse>>());
    }

    /// <summary>Получить список опубликованных статей</summary>
    /// <response code="200">Список опубликованных статей</response>
    [HttpGet("published")]
    [ProducesResponseType(typeof(List<ArticlePreviewResponse>), 200)]
    public async Task<IActionResult> GetPublishedArticles(
        [FromQuery] PageOptions pageOptions,
        CancellationToken cancellationToken)
    {
        var publishedArticles = await articleService.GetPublishedArticles(pageOptions, cancellationToken);

        Response.Headers.Append("X-Total-Count", publishedArticles.TotalCount.ToString());
        return Ok(publishedArticles.Adapt<List<ArticlePreviewResponse>>());
    }

    /// <summary>Получить статью</summary>
    /// <param name="articleId">Id статьи, которую нужно получить</param>
    /// <response code="200">Статья получена</response>
    /// <response code="404">Статья не найдена</response>
    [HttpGet("{articleId}")]
    [ProducesResponseType(typeof(ArticleResponse), 200)]
    public async Task<IActionResult> GetArticle([Required] string articleId)
    {
        var getArticleResult = await articleService.GetArticle(articleId);

        if (getArticleResult.IsError)
            return Problem(getArticleResult.Errors);

        var authResult = await authService.AuthorizeAsync(User, "CanManageArticles");
        if (getArticleResult.Value.IsPublished == false && !authResult.Succeeded)
            getArticleResult = Article.Errors.NotFound;

        return getArticleResult.Match(article => Ok(article.Adapt<ArticleResponse>()), Problem);
    }

    /// <summary>Обновить статью</summary>
    /// <response code="204">Статья обновлена</response>
    /// <response code="400">
    /// Id статьи указан некорректно;
    /// Заголовок статьи указан некорректно;
    /// Описание статьи указано некорректно
    /// </response>
    /// <response code="404">Статья не найдена</response>
    [HttpPut, Authorize(Policy = "CanManageArticles")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> UpdateArticle([Required] UpdateArticleRequest request)
    {
        var requestToArticleResult = CreateArticleFrom(request);

        if (requestToArticleResult.IsError)
            return Problem(requestToArticleResult.Errors);

        var article = requestToArticleResult.Value;
        var updateArticleResult = await articleService.UpdateArticle(article);

        return updateArticleResult.Match(_ => NoContent(), Problem);
    }

    /// <summary>Закрепить/открепить статью</summary>
    /// <param name="articleId">Id статьи, которую нужно закрепить/открепить</param>
    /// <response code="204">Статья закреплена/откреплена</response>
    /// <response code="404">Статья не найдена</response>
    [HttpPatch("{articleId}/pin"), Authorize(Policy = "CanManageArticles")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> ChangePinState([Required] string articleId)
    {
        var changePinStateResult = await articleService.ChangePinState(articleId);

        return changePinStateResult.Match(_ => NoContent(), Problem);
    }

    /// <summary>Увеличить счетчик просмотров статьи на единицу</summary>
    /// <param name="articleId">Id статьи, которой нужно увеличить счетчик просмотров</param>
    /// <response code="204">Счетчик просмотров увеличен</response>
    /// <response code="404">Статья не найдена; У статьи не найден счетчик просмотров</response>
    [HttpPatch("{articleId}/increase-views")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> IncreaseViewsCounter([Required] string articleId)
    {
        var increaseViewsCounterResult = await articleService.IncreaseViewsCounter(articleId);

        return increaseViewsCounterResult.Match(_ => NoContent(), Problem);
    }

    /// <summary>Удалить статью</summary>
    /// <param name="articleId">Id статьи, которую нужно удалить</param>
    /// <response code="204">Статья удалена</response>
    /// <response code="404">Статья не найдена</response>
    [HttpDelete("{articleId}"), Authorize(Policy = "CanManageArticles")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> DeleteArticle([Required] string articleId)
    {
        var deleteArticleResult = await articleService.DeleteArticle(articleId);

        return deleteArticleResult.Match(_ => NoContent(), Problem);
    }

    private static ErrorOr<Article> CreateArticleFrom(CreateArticleRequest request) =>
        Article.Create(
            request.ArticleId,
            request.Title,
            request.Description,
            request.Text,
            request.IsPublished,
            request.IsPinned);

    private static ErrorOr<Article> CreateArticleFrom(UpdateArticleRequest request) =>
        Article.Create(
            request.ArticleId,
            request.Title,
            request.Description,
            request.Text,
            request.IsPublished,
            request.IsPinned);

    private CreatedAtActionResult CreatedAtGetArticle(Article article) =>
        CreatedAtAction(
            actionName: nameof(GetArticle),
            routeValues: new { articleId = article.ArticleId },
            value: article.Adapt<ArticleResponse>());
}