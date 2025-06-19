namespace WebAPI.Controllers;

[Route("/api/collections")]
public sealed class CollectionsController(
    ICollectionRepository collectionRepository,
    IArticleRepository articleRepository)
    : ApiController
{
    /// <summary>Создать коллекцию</summary>
    /// <response code="201">Коллекция создана</response>
    /// <response code="400">
    /// Id коллекции указан некорректно;
    /// Название коллекции указано некорректно
    /// </response>
    [HttpPost, Authorize(Policy = "CanManageArticles")]
    [ProducesResponseType(typeof(CollectionPreviewResponse), 201)]
    public async Task<IActionResult> CreateCollectionAsync([Required] CreateCollectionRequest request)
    {
        var requestToCollectionResult = CreateCollectionFrom(request);

        if (requestToCollectionResult.IsError)
            return Problem(requestToCollectionResult.Errors);

        var collection = requestToCollectionResult.Value;

        if (await collectionRepository.IsCollectionExistsAsync(collection.CollectionId))
            return Problem([ Collection.Errors.NonUniqueId ]);

        await collectionRepository.AddCollectionAsync(collection);
        await collectionRepository.SaveChangesAsync();

        return CreatedAtGetCollection(collection);
    }

    /// <summary>Добавить статью в коллекцию</summary>
    /// <response code="204">Статья добавлена в коллекцию</response>
    /// <response code="400">Статья уже была в коллекции</response>
    /// <response code="404">Коллекция не найдена; Статья не найдена</response>
    [HttpPost("{collectionId}"), Authorize(Policy = "CanManageArticles")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> AddArticleToCollectionAsync(
        [Required] string collectionId,
        [Required] AddArticleToCollectionRequest request)
    {
        var collection = await collectionRepository.FindCollectionByIdAsync(collectionId);

        if (collection.IsError)
            return Problem(collection.Errors);

        var article = await articleRepository.FindArticleByIdAsync(request.ArticleId);

        if (article.IsError)
            return Problem(article.Errors);

        if (collection.Value.Articles.Contains(article.Value))
            return Problem([ Collection.Errors.ArticleAlreadyAdded ]);

        collection.Value.AddArticle(article.Value);
        await collectionRepository.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>Получить список коллекций</summary>
    /// <response code="200">Список коллекций</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<CollectionPreviewResponse>), 200)]
    public async Task<IActionResult> GetCollectionsAsync()
    {
        var collections = await collectionRepository.GetCollectionsAsync();

        return Ok(collections.MapToCollectionPreviewResponseList());
    }

    /// <summary>Получить коллекцию со статьями</summary>
    /// <response code="200">Коллекция получена</response>
    /// <response code="404">Коллекция не найдена</response>
    [HttpGet("{collectionId}"), Authorize(Policy = "CanManageArticles")]
    [ActionName(nameof(GetCollectionAsync))]
    [ProducesResponseType(typeof(CollectionResponse), 200)]
    public async Task<IActionResult> GetCollectionAsync([Required] string collectionId)
    {
        var getCollectionResult = await collectionRepository.FindCollectionByIdAsync(collectionId);

        return getCollectionResult.Match(collection => Ok(collection.MapToCollectionResponse()), Problem);
    }

    /// <summary>Получить список опубликованных статей из коллекции</summary>
    /// <response code="200">Список опубликованных статей из коллекции</response>
    /// <response code="404">Коллекция не найдена</response>
    [HttpGet("{collectionId}/published")]
    [ProducesResponseType(typeof(List<ArticlePreviewResponse>), 200)]
    public async Task<IActionResult> GetPublishedArticlesFromCollectionAsync(
        [Required] string collectionId,
        [FromQuery] PageOptions pageOptions,
        CancellationToken cancellationToken)
    {
        var getPublishedArticlesFromCollectionResult =
            await collectionRepository.GetPublishedArticlesFromColelctionAsync(
                collectionId,
                pageOptions,
                cancellationToken);

        if (getPublishedArticlesFromCollectionResult.IsError)
            return Problem(getPublishedArticlesFromCollectionResult.Errors);

        var publishedArticlesFromCollection = getPublishedArticlesFromCollectionResult.Value;

        Response.Headers.Append("X-Total-Count", publishedArticlesFromCollection.TotalCount.ToString());
        return Ok(publishedArticlesFromCollection.MapToArticlePreviewResponseList());
    }

    /// <summary>Обновить коллекцию</summary>
    /// <response code="204">Коллекция обновлена</response>
    /// <response code="400">Название коллекции указано некорректно</response>
    /// <response code="404">Коллекция не найдена</response>
    [HttpPut, Authorize(Policy = "CanManageArticles")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> UpdateCollectionAsync([Required] UpdateCollectionRequest request)
    {
        var requestToCollectionResult = CreateCollectionFrom(request);

        if (requestToCollectionResult.IsError)
            return Problem(requestToCollectionResult.Errors);

        var updatedCollection = requestToCollectionResult.Value;

        var collection = await collectionRepository.FindCollectionByIdAsync(updatedCollection.CollectionId);

        if (collection.IsError)
            return Problem(collection.Errors);

        collection.Value.Update(updatedCollection);
        await collectionRepository.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>Удалить коллекцию</summary>
    /// <response code="204">Коллекция удалена</response>
    /// <response code="404">Коллекция не найдена</response>
    [HttpDelete("{collectionId}"), Authorize(Policy = "CanManageArticles")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> DeleteCollectionAsync([Required] string collectionId)
    {
        var deleteCollectionResult = await collectionRepository.DeleteCollectionAsync(collectionId);

        if (deleteCollectionResult.IsError)
            return Problem(deleteCollectionResult.Errors);

        await collectionRepository.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>Удалить статью из коллекцию</summary>
    /// <response code="204">Статья удалена из коллекции</response>
    /// <response code="404">Коллекция не найдена; Статья в коллекции не найдена</response>
    [HttpDelete("{collectionId}/articles/{articleId}"), Authorize(Policy = "CanManageArticles")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> DeleteArticleFromCollectionAsync(
        [Required] string collectionId,
        [Required] string articleId)
    {
        var collection = await collectionRepository.FindCollectionByIdAsync(collectionId);

        if (collection.IsError)
            return Problem(collection.Errors);

        var result = collection.Value.DeleteArticle(articleId);

        if (result.IsError)
            return Problem(result.Errors);

        await collectionRepository.SaveChangesAsync();

        return NoContent();
    }

    private static ErrorOr<Collection> CreateCollectionFrom(CreateCollectionRequest request)
    {
        return Collection.Create(
            collectionId: request.CollectionId,
            title: request.Title);
    }

    private static ErrorOr<Collection> CreateCollectionFrom(UpdateCollectionRequest request)
    {
        return Collection.Create(
            collectionId: request.CollectionId,
            title: request.Title);
    }

    private CreatedAtActionResult CreatedAtGetCollection(Collection collection)
    {
        return CreatedAtAction(
            actionName: nameof(GetCollectionAsync),
            routeValues: new { collectionId = collection.CollectionId },
            value: collection.MapToCollectionPreviewResponse());
    }
}
