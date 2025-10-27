namespace WebAPI.IntegrationTests.ArticlesTests;

[Collection("Tests")]
public sealed class GetArticlesTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
    private readonly CustomWebApplicationFactory _factory = factory;

    [Fact]
    public async Task EmptyListWillBeReturnedIfNoArticlesExist()
    {
        // Arrange
        var gqlClient = _factory.AuthorizeAs(UserTypes.Admin).CreateClient().ToGqlClient();

        // Act
        var gqlResponse = await gqlClient.GetArticles(new PageOptions { Page = 1, Size = 10 });

        // Assert
        gqlResponse.ShouldBeEmpty();
    }

    [Fact]
    public async Task ArticlesWillBeReturnedIfArticlesExist()
    {
        // Arrange
        var gqlClient = _factory.AuthorizeAs(UserTypes.Admin).CreateClient().ToGqlClient();
        const int numberOfArticles = 2;

        for (var index = 1; index <= numberOfArticles; index++)
        {
            await gqlClient.CreateArticle(Inputs.Article.ArticleInput with { ArticleId = $"article-{index}" });
        }
        
        // Act
        var gqlResponse = await gqlClient.GetArticles(new PageOptions { Page = 1, Size = 10 });

        // Assert
        gqlResponse.Count.ShouldBe(numberOfArticles);
    }

    [Fact]
    public async Task OnlyAuthorizedUserCanReadArticles()
    {
        // Arrange
        var gqlClient = _factory.CreateClient().ToGqlClient();
        
        // Act
        var exception = await Should.ThrowAsync<GraphQLException>(async () =>
        {
            await gqlClient.GetArticles(new PageOptions { Page = 1, Size = 10 });
        });
        
        // Assert
        exception.Message!.ShouldContain("The current user is not authorized to access this resource.");
    }

    [Fact]
    public async Task OnlyUserWithCanManageArticlesClaimCanReadArticles()
    {
        // Arrange
        var gqlClient = _factory.AuthorizeAs(UserTypes.User).CreateClient().ToGqlClient();
        
        // Act
        var exception = await Should.ThrowAsync<GraphQLException>(async () =>
        {
            await gqlClient.GetArticles(new PageOptions { Page = 1, Size = 10 });
        });
        
        // Assert
        exception.Message!.ShouldContain("The current user is not authorized to access this resource.");
    }

    [Fact]
    public async Task PaginationShouldWork()
    {
        // Arrange
        var gqlClient = _factory.AuthorizeAs(UserTypes.Admin).CreateClient().ToGqlClient();
        const int numberOfDrafts = 15;
        const int numberOfPublishedArticles = 1;
        var publishedArticleId = $"article-{numberOfDrafts + numberOfPublishedArticles}";

        for (var index = 1; index <= numberOfDrafts; index++)
        {
            await gqlClient.CreateArticle(Inputs.Article.ArticleInput with
            {
                ArticleId = $"article-{index}",
                IsPublished = false
            });
        }
        
        await gqlClient.CreateArticle(Inputs.Article.ArticleInput with
        {
            ArticleId = publishedArticleId,
            IsPublished = true
        });

        // Act
        var gqlResponse1 = await gqlClient.GetArticles(new PageOptions { Page = 1, Size = 10 });
        var gqlResponse2 = await gqlClient.GetArticles(new PageOptions { Page = 1, Size = 5 });
        var gqlResponse3 = await gqlClient.GetArticles(new PageOptions { Page = 2, Size = 10 });
        var gqlResponse4 = await gqlClient.GetArticles(new PageOptions { Page = 3, Size = 10 });
       
        // Assert
        gqlResponse1.Count.ShouldBe(10);
        gqlResponse2.Count.ShouldBe(5);
        gqlResponse3.Count.ShouldBe(6);
        gqlResponse4.Count.ShouldBe(0);
    }
}
