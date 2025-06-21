namespace WebAPI.Controllers;

[Route("/api/articles")]
public sealed class ArticlesController(IArticleRepository articleRepository, IAuthorizationService authService)
    : ApiController
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
    public async Task<IActionResult> CreateArticleAsync([Required] CreateArticleRequest request)
    {
        var requestToArticleResult = CreateArticleFrom(request);

        if (requestToArticleResult.IsError)
            return Problem(requestToArticleResult.Errors);

        var article = requestToArticleResult.Value;

        if (await articleRepository.IsArticleExistsAsync(article.ArticleId))
            return Problem([ Article.Errors.NonUniqueId ]);

        await articleRepository.AddArticleAsync(article);
        await articleRepository.SaveChangesAsync();

        return CreatedAtGetArticleAsync(article);
    }

    /// <summary>Получить список статей</summary>
    /// <response code="200">Список статей</response>
    [HttpGet, Authorize(Policy = "CanManageArticles")]
    [ProducesResponseType(typeof(List<ArticlePreviewResponse>), 200)]
    public async Task<IActionResult> GetArticlesAsync(
        [FromQuery] PageOptions pageOptions,
        CancellationToken cancellationToken)
    {
        var articles = await articleRepository.GetArticlesAsync(pageOptions, cancellationToken);

        Response.Headers.Append("X-Total-Count", articles.TotalCount.ToString());
        return Ok(articles.MapToArticlePreviewResponseList());
    }

    /// <summary>Получить список опубликованных статей</summary>
    /// <response code="200">Список опубликованных статей</response>
    [HttpGet("published")]
    [ProducesResponseType(typeof(List<ArticlePreviewResponse>), 200)]
    public async Task<IActionResult> GetPublishedArticlesAsync(
        [FromQuery] PageOptions pageOptions,
        CancellationToken cancellationToken)
    {
        var publishedArticles = await articleRepository.GetPublishedArticlesAsync(pageOptions, cancellationToken);

        Response.Headers.Append("X-Total-Count", publishedArticles.TotalCount.ToString());
        return Ok(publishedArticles.MapToArticlePreviewResponseList());
    }

    /// <summary>Получить статью</summary>
    /// <param name="articleId">Id статьи, которую нужно получить</param>
    /// <response code="200">Статья получена</response>
    /// <response code="404">Статья не найдена</response>
    [HttpGet("{articleId}")]
    [ActionName(nameof(GetArticleAsync))]
    [ProducesResponseType(typeof(ArticleResponse), 200)]
    public async Task<IActionResult> GetArticleAsync([Required] string articleId)
    {
        var getArticleResult = await articleRepository.FindArticleByIdAsync(articleId);

        if (getArticleResult.IsError)
            return Problem(getArticleResult.Errors);

        var authResult = await authService.AuthorizeAsync(User, "CanManageArticles");
        if (getArticleResult.Value.IsPublished == false && !authResult.Succeeded)
            return Problem([ Article.Errors.NotFound ]);

        var article = getArticleResult.Value;

        return Ok(article.MapToArticleResponse());
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
    public async Task<IActionResult> UpdateArticleAsync([Required] UpdateArticleRequest request)
    {
        var requestToArticleResult = CreateArticleFrom(request);

        if (requestToArticleResult.IsError)
            return Problem(requestToArticleResult.Errors);

        var updatedArticle = requestToArticleResult.Value;

        var article = await articleRepository.FindArticleByIdAsync(updatedArticle.ArticleId);

        if (article.IsError)
            return Problem(article.Errors);

        article.Value.Update(updatedArticle);
        await articleRepository.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>Закрепить/открепить статью</summary>
    /// <param name="articleId">Id статьи, которую нужно закрепить/открепить</param>
    /// <response code="204">Статья закреплена/откреплена</response>
    /// <response code="404">Статья не найдена</response>
    [HttpPatch("{articleId}/pin"), Authorize(Policy = "CanManageArticles")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> ChangePinStateAsync([Required] string articleId)
    {
        var article = await articleRepository.FindArticleByIdAsync(articleId);

        if (article.IsError)
            return Problem(article.Errors);

        article.Value.ChangePinState();
        await articleRepository.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>Увеличить счетчик просмотров статьи на единицу</summary>
    /// <param name="articleId">Id статьи, которой нужно увеличить счетчик просмотров</param>
    /// <response code="204">Счетчик просмотров увеличен</response>
    /// <response code="404">Статья не найдена</response>
    [HttpPatch("{articleId}/increase-views")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> IncreaseViewsCounterAsync([Required] string articleId)
    {
        var article = await articleRepository.FindArticleByIdAsync(articleId);

        if (article.IsError)
            return Problem(article.Errors);

        article.Value.IncreaseViewsCounter();
        await articleRepository.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>Удалить статью</summary>
    /// <param name="articleId">Id статьи, которую нужно удалить</param>
    /// <response code="204">Статья удалена</response>
    /// <response code="404">Статья не найдена</response>
    [HttpDelete("{articleId}"), Authorize(Policy = "CanManageArticles")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> DeleteArticleAsync([Required] string articleId)
    {
        var result = await articleRepository.DeleteArticleAsync(articleId);

        if (result.IsError)
            return Problem(result.Errors);

        await articleRepository.SaveChangesAsync();

        return NoContent();
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

    private CreatedAtActionResult CreatedAtGetArticleAsync(Article article)
    {
        return CreatedAtAction(
            actionName: nameof(GetArticleAsync),
            routeValues: new { articleId = article.ArticleId },
            value: article.MapToArticleResponse());
    }
}
