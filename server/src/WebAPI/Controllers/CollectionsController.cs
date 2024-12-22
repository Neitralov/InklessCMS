namespace WebAPI.Controllers;

[Route("/api/collections")]
public sealed class CollectionsController(CollectionService collectionService) : ApiController
{
    /// <summary>Создать коллекцию</summary>
    /// <response code="201">Коллекция создана</response>
    /// <response code="400">
    /// Id коллекции указан некорректно;
    /// Название коллекции указано некорректно
    /// </response>
    [HttpPost, Authorize(Policy = "CanManageArticles")]
    [ProducesResponseType(typeof(CollectionPreviewResponse), 201)]
    public async Task<IActionResult> CreateCollection([Required] CreateCollectionRequest request)
    {
        var requestToCollectionResult = CreateCollectionFrom(request);

        if (requestToCollectionResult.IsError)
            return Problem(requestToCollectionResult.Errors);

        var collection = requestToCollectionResult.Value;
        var createCollectionResult = await collectionService.AddCollection(collection);

        return createCollectionResult.Match(_ => CreatedAtGetCollection(collection), Problem);
    }

    /// <summary>Добавить статью в коллекцию</summary>
    /// <response code="204">Статья добавлена в коллекцию</response>
    /// <response code="400">Статья уже была в коллекции</response>
    /// <response code="404">Коллекция не найдена; Статья не найдена</response>
    [HttpPost("{collectionId}"), Authorize(Policy = "CanManageArticles")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> AddArticleToCollection(
        [Required] string collectionId,
        [Required] AddArticleToCollectionRequest request)
    {
        var requestToAddArticleInCollectionResult =
            await collectionService.AddArticleToCollection(collectionId, request.ArticleId);

        return requestToAddArticleInCollectionResult.Match(_ => NoContent(), Problem);
    }

    /// <summary>Получить список коллекций</summary>
    /// <response code="200">Список коллекций</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<CollectionPreviewResponse>), 200)]
    public async Task<IActionResult> GetCollections()
    {
        var collections = await collectionService.GetCollections();

        return Ok(collections.Adapt<List<CollectionPreviewResponse>>());
    }

    /// <summary>Получить коллекцию со статьями</summary>
    /// <response code="200">Коллекция получена</response>
    /// <response code="404">Коллекция не найдена</response>
    [HttpGet("{collectionId}"), Authorize(Policy = "CanManageArticles")]
    [ProducesResponseType(typeof(List<CollectionResponse>), 200)]
    public async Task<IActionResult> GetCollection([Required] string collectionId)
    {
        var getCollectionResult = await collectionService.GetCollection(collectionId);

        return getCollectionResult.Match(collection => Ok(collection.Adapt<CollectionResponse>()), Problem);
    }

    /// <summary>Получить список опубликованных статей из коллекции</summary>
    /// <response code="200">Список опубликованных статей из коллекции</response>
    /// <response code="404">Коллекция не найдена</response>
    [HttpGet("{collectionId}/published")]
    [ProducesResponseType(typeof(List<ArticlePreviewResponse>), 200)]
    public async Task<IActionResult> GetPublishedArticlesFromCollection(
        [Required] string collectionId,
        [FromQuery] PageOptions pageOptions,
        CancellationToken cancellationToken)
    {
        var getPublishedArticlesFromCollectionResult =
            await collectionService.GetPublishedArticlesFromCollection(collectionId, pageOptions, cancellationToken);

        if (getPublishedArticlesFromCollectionResult.IsError)
            return Problem(getPublishedArticlesFromCollectionResult.Errors);

        var publishedArticlesFromCollection = getPublishedArticlesFromCollectionResult.Value;

        Response.Headers.Append("X-Total-Count", publishedArticlesFromCollection.TotalCount.ToString());
        return Ok(publishedArticlesFromCollection.Adapt<List<ArticlePreviewResponse>>());
    }

    /// <summary>Обновить коллекцию</summary>
    /// <response code="204">Коллекция обновлена</response>
    /// <response code="400">Название коллекции указано некорректно</response>
    /// <response code="404">Коллекция не найдена</response>
    [HttpPut, Authorize(Policy = "CanManageArticles")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> UpdateCollection([Required] UpdateCollectionRequest request)
    {
        var requestToCollectionResult = CreateCollectionFrom(request);

        if (requestToCollectionResult.IsError)
            return Problem(requestToCollectionResult.Errors);

        var collection = requestToCollectionResult.Value;
        var updateCollectionResult = await collectionService.UpdateCollection(collection);

        return updateCollectionResult.Match(_ => NoContent(), Problem);
    }

    /// <summary>Удалить коллекцию</summary>
    /// <response code="204">Коллекция удалена</response>
    /// <response code="404">Коллекция не найдена</response>
    [HttpDelete("{collectionId}"), Authorize(Policy = "CanManageArticles")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> DeleteCollection([Required] string collectionId)
    {
        var deleteCollectionResult = await collectionService.DeleteCollection(collectionId);

        return deleteCollectionResult.Match(_ => NoContent(), Problem);
    }

    /// <summary>Удалить статью из коллекцию</summary>
    /// <response code="204">Статья удалена из коллекции</response>
    /// <response code="404">Коллекция не найдена; Статья в коллекции не найдена</response>
    [HttpDelete("{collectionId}/articles/{articleId}"), Authorize(Policy = "CanManageArticles")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> DeleteArticleFromCollection(
        [Required] string collectionId,
        [Required] string articleId)
    {
        var deleteArticleFromCollectionResult =
            await collectionService.DeleteArticleFromCollection(collectionId, articleId);

        return deleteArticleFromCollectionResult.Match(_ => NoContent(), Problem);
    }

    private static ErrorOr<Collection> CreateCollectionFrom(CreateCollectionRequest request) =>
        Collection.Create(
            collectionId: request.CollectionId,
            title: request.Title);

    private static ErrorOr<Collection> CreateCollectionFrom(UpdateCollectionRequest request) =>
        Collection.Create(
            collectionId: request.CollectionId,
            title: request.Title);

    private CreatedAtActionResult CreatedAtGetCollection(Collection collection) =>
        CreatedAtAction(
            actionName: nameof(GetCollection),
            routeValues: new { collectionId = collection.CollectionId },
            value: collection.Adapt<CollectionPreviewResponse>());
}