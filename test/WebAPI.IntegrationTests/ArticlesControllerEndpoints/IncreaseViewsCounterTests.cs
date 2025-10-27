using Domain.Articles;

namespace WebAPI.IntegrationTests.ArticlesControllerEndpoints;

[Collection("Tests")]
public sealed class IncreaseViewsCounterTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
    private readonly CustomWebApplicationFactory _factory = factory;

    [Fact]
    public async Task ViewsCounterCanBeIncreased()
    {
        // Arrange
        var gqlClient = _factory.CreateClient().ToGqlClient();
        var adminGqlClient = _factory.AuthorizeAs(UserTypes.Admin).CreateClient().ToGqlClient();
        const string articleId = "article-id";
        const int totalViews = 2;

        await adminGqlClient.CreateArticle(Inputs.Article.ArticleInput with { ArticleId = articleId });

        // Act
        await gqlClient.IncreaseViews(articleId);
        var gqlResponse = await gqlClient.IncreaseViews(articleId);

        // Assert
        gqlResponse.Views.ShouldBe(totalViews);
    }

    [Fact]
    public async Task ViewsCounterCannotBeIncreasedIfArticleDoesNotExist()
    {
        // Arrange
        var gqlClient = _factory.CreateClient().ToGqlClient();
        const string articleId = "article-id";

        // Act
        var exception = await Should.ThrowAsync<GraphQLException>(async () =>
        {
            await gqlClient.IncreaseViews(articleId);
        });

        // Assert
        exception.Message!.ShouldContain(Article.Errors.NotFound.Code);
    }
}
