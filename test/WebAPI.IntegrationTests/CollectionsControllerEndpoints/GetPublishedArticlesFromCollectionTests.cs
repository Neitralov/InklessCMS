namespace WebAPI.IntegrationTests.CollectionsControllerEndpoints;

[Collection("Tests")]
public sealed class GetPublishedArticlesFromCollectionTests(CustomWebApplicationFactory factory)
    : BaseIntegrationTest(factory)
{
    private readonly CustomWebApplicationFactory _factory = factory;

    [Fact]
    public async Task EmptyListWillBeReturnedIfCollectionDoesNotContainPublishedArticles()
    {
        // Arrange
        var gqlAdminClient = _factory.AuthorizeAs(UserTypes.Admin).CreateClient().ToGqlClient();
        const string collectionId = "collection-id";

        await gqlAdminClient.CreateCollection(Requests.Collection.CollectionInput with { CollectionId = collectionId });

        var gqlClient = _factory.CreateClient().ToGqlClient();

        // Act
        var gqlResponse = await gqlClient.GetPublishedArticlesFromCollection(collectionId, new PageOptions { Page = 1, Size = 10 });

        // Assert
        gqlResponse.ShouldBeEmpty();
    }

    [Fact]
    public async Task PublishedArticlesWillBeReturnedIfCollectionContainsPublishedArticles()
    {
        // Arrange
        var gqlAdminClient = _factory.AuthorizeAs(UserTypes.Admin).CreateClient().ToGqlClient();
        const string collectionId = "collection-id";
        const int numberOfPublishedArticles = 1;

        await gqlAdminClient.CreateCollection(Requests.Collection.CollectionInput with { CollectionId = collectionId });

        const string firstArticleId = "article-1";
        await gqlAdminClient.CreateArticle(Requests.Article.ArticleInput with { ArticleId = firstArticleId, IsPublished = true });
        await gqlAdminClient.AddArticleToCollection(collectionId, firstArticleId);

        const string secondArticleId = "article-2";
        await gqlAdminClient.CreateArticle(Requests.Article.ArticleInput with { ArticleId = secondArticleId, IsPublished = false });
        await gqlAdminClient.AddArticleToCollection(collectionId, secondArticleId);

        var gqlClient = _factory.CreateClient().ToGqlClient();

        // Act
        var gqlResponse = await gqlClient.GetPublishedArticlesFromCollection(collectionId, new PageOptions { Page = 1, Size = 10 });

        // Assert
        gqlResponse.Count.ShouldBe(numberOfPublishedArticles);
    }

    [Fact]
    public async Task PublishedArticlesWontBeReturnedIfCollectionDoesNotExist()
    {
        // Arrange
        var gqlClient = _factory.CreateClient().ToGqlClient();
        const string collectionId = "collection-id";

        // Act
        var exception = await Should.ThrowAsync<GraphQLException>(async () =>
        {
            await gqlClient.GetPublishedArticlesFromCollection(collectionId, new PageOptions { Page = 1, Size = 10 });
        });

        // Assert
        exception.Message!.ShouldContain(Collection.Errors.NotFound.Code);
    }

    [Fact]
    public async Task PaginationShouldWork()
    {
        // Arrange
        var gqlAdminClient = _factory.AuthorizeAs(UserTypes.Admin).CreateClient().ToGqlClient();
        const string collectionId = "collection-id";

        await gqlAdminClient.CreateCollection(Requests.Collection.CollectionInput with { CollectionId = collectionId });

        const int numberOfPublishedArticles = 15;
        const int numberOfDrafts = 1;
        var draftArticleId = $"article-{numberOfPublishedArticles + numberOfDrafts}";

        for (var index = 1; index <= numberOfPublishedArticles; index++)
            await gqlAdminClient.CreateArticle(Requests.Article.ArticleInput with
            {
                ArticleId = $"article-{index}",
                IsPublished = true
            });
            
        for (var index = 1; index <= numberOfPublishedArticles; index++)
            await gqlAdminClient.AddArticleToCollection(collectionId, $"article-{index}");

        await gqlAdminClient.CreateArticle(Requests.Article.ArticleInput with
        {
            ArticleId = draftArticleId,
            IsPublished = false
        });
            
        await gqlAdminClient.AddArticleToCollection(collectionId, draftArticleId);

        var gqlClient = _factory.CreateClient().ToGqlClient();

        // Act
        var gqlResponse1 =
            await gqlClient.GetPublishedArticlesFromCollection(collectionId, new PageOptions { Page = 1, Size = 10 });
        var gqlResponse2 =
            await gqlClient.GetPublishedArticlesFromCollection(collectionId, new PageOptions { Page = 1, Size = 5 });
        var gqlResponse3 =
            await gqlClient.GetPublishedArticlesFromCollection(collectionId, new PageOptions { Page = 2, Size = 10 });
        var gqlResponse4 = 
            await gqlClient.GetPublishedArticlesFromCollection(collectionId, new PageOptions { Page = 3, Size = 10 });

        // Assert
        gqlResponse1.Count.ShouldBe(10);
        gqlResponse2.Count.ShouldBe(5);
        gqlResponse3.Count.ShouldBe(5);
        gqlResponse4.Count.ShouldBe(0);
    }
}
