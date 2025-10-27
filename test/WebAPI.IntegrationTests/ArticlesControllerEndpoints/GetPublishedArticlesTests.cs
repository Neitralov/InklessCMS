namespace WebAPI.IntegrationTests.ArticlesControllerEndpoints;

[Collection("Tests")]
public sealed class GetPublishedArticlesTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
    private readonly CustomWebApplicationFactory _factory = factory;

    [Fact]
    public async Task EmptyListWillBeReturnedIfNoPublishedArticlesExist()
    {
        // Arrange
        var gqlClient = _factory.CreateClient().ToGqlClient();
        var adminGqlClient = _factory.AuthorizeAs(UserTypes.Admin).CreateClient().ToGqlClient();

        await adminGqlClient.CreateArticle(Inputs.Article.ArticleInput with { IsPublished = false });

        // Act
        var gqlResponse = await gqlClient.GetPublishedArticles(new PageOptions { Page = 1, Size = 10 });

        // Assert
        gqlResponse.ShouldBeEmpty();
    }

    [Fact]
    public async Task PublishedArticlesWillBeReturnedIfPublishedArticlesExist()
    {
        // Arrange
        var gqlClient = _factory.CreateClient().ToGqlClient();
        var adminGqlClient = _factory.AuthorizeAs(UserTypes.Admin).CreateClient().ToGqlClient();
        const int numberOfPublishedArticles = 1;

        const string firstArticleId = "article-1";
        await adminGqlClient.CreateArticle(Inputs.Article.ArticleInput with
        {
            ArticleId = firstArticleId, 
            IsPublished = false
        });

        const string secondArticleId = "article-2";
        await adminGqlClient.CreateArticle(Inputs.Article.ArticleInput with
        {
            ArticleId = secondArticleId, 
            IsPublished = true
        });

        // Act
        var gqlResponse = await gqlClient.GetPublishedArticles(new PageOptions { Page = 1, Size = 10 });

        // Assert
        gqlResponse.Count.ShouldBe(numberOfPublishedArticles);
    }

    [Fact]
    public async Task PaginationShouldWork()
    {
        // Arrange
        var gqlClient = _factory.CreateClient().ToGqlClient();
        var adminGqlClient = _factory.AuthorizeAs(UserTypes.Admin).CreateClient().ToGqlClient();
        const int numberOfPublishedArticles = 15;
        const int numberOfDrafts = 1;
        var draftArticleId = $"article-{numberOfPublishedArticles + numberOfDrafts}";

        for (var index = 1; index <= numberOfPublishedArticles; index++)
            await adminGqlClient.CreateArticle(Inputs.Article.ArticleInput with
            {
                ArticleId = $"article-{index}",
                IsPublished = true
            });

        await adminGqlClient.CreateArticle(Inputs.Article.ArticleInput with
        {
            ArticleId = draftArticleId, 
            IsPublished = false
        });

        // Act
        var gqlResponse1 = await gqlClient.GetPublishedArticles(new PageOptions { Page = 1, Size = 10 });
        var gqlResponse2 = await gqlClient.GetPublishedArticles(new PageOptions { Page = 1, Size = 5 });
        var gqlResponse3 = await gqlClient.GetPublishedArticles(new PageOptions { Page = 2, Size = 10 });
        var gqlResponse4 = await gqlClient.GetPublishedArticles(new PageOptions { Page = 3, Size = 10 });
        
        // Assert
        gqlResponse1.Count.ShouldBe(10);
        gqlResponse2.Count.ShouldBe(5);
        gqlResponse3.Count.ShouldBe(5);
        gqlResponse4.Count.ShouldBe(0);
    }
}
