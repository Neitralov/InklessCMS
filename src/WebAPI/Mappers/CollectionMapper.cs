public static class CollectionMapper
{
    public static CollectionResponse MapToCollectionResponse(this Collection collection)
    {
        return new CollectionResponse(
            collection.CollectionId,
            collection.Title,
            collection.Articles.Select(article => article.MapToArticlePreviewResponse()).ToList()
        );
    }

    public static CollectionPreviewResponse MapToCollectionPreviewResponse(this Collection collection)
    {
        return new CollectionPreviewResponse(
            collection.CollectionId,
            collection.Title
        );
    }

    public static List<CollectionPreviewResponse> MapToCollectionPreviewResponseList(this List<Collection> collections)
    {
        return collections
            .Select(collection => collection.MapToCollectionPreviewResponse())
            .ToList();
    }
}
