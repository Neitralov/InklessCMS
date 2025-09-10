using Domain.Articles;

namespace WebAPI.IntegrationTests.ArticlesControllerEndpoints;

[Collection("Tests")]
public sealed class GetArticleTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
    private readonly CustomWebApplicationFactory _factory = factory;

    [Fact]
    public async Task ArticleWillBeReturnedIfItExists()
    {
        // Arrange
        var gqlClient = _factory.CreateClient().ToGqlClient();
        var adminGqlClient = _factory.AuthorizeAs(UserTypes.Admin).CreateClient().ToGqlClient();

        const string firstArticleId = "article-1";
        await adminGqlClient.CreateArticle(Requests.Article.ArticleInput with
        {
            ArticleId = firstArticleId,
            IsPublished = false
        });

        const string secondArticleId = "article-2";
        await adminGqlClient.CreateArticle(Requests.Article.ArticleInput with
        {
            ArticleId = secondArticleId,
            IsPublished = true
        });

        // Act
        var gqlResponse1 = await adminGqlClient.GetArticle(firstArticleId);
        var gqlResponse2 = await gqlClient.GetArticle(secondArticleId);

        // Assert
        gqlResponse1.ArticleId.ShouldBe(firstArticleId);
        gqlResponse2.ArticleId.ShouldBe(secondArticleId);
    }

    [Fact]
    public async Task ArticleWontBeReturnedIfItDoesNotExist()
    {
        // Arrange
        var gqlClient = _factory.CreateClient().ToGqlClient();
        const string articleId = "article-id";

        // Act
        var exception = await Should.ThrowAsync<GraphQLHttpRequestException>(async () =>
        {
            await gqlClient.GetArticle(articleId);
        });
        
        // Assert
        exception.Content!.ShouldContain(Article.Errors.NotFound.Code);
    }

    [Fact]
    public async Task DraftWontBeReturnedIfYouHaveNoCanManageArticlesClaim()
    {
        // Arrange
        var gqlClient = _factory.CreateClient().ToGqlClient();
        var adminGqlClient = _factory.AuthorizeAs(UserTypes.Admin).CreateClient().ToGqlClient();
        
        const string articleId = "article-id";
        await adminGqlClient.CreateArticle(Requests.Article.ArticleInput with
        {
            ArticleId = articleId,
            IsPublished = false
        });

        // Act
        var exception = await Should.ThrowAsync<GraphQLHttpRequestException>(async () =>
        {
            await gqlClient.GetArticle(articleId);
        });
        
        // Assert
        exception.Content!.ShouldContain(Article.Errors.NotFound.Code);
    }
}
