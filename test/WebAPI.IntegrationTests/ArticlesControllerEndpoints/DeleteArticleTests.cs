using Domain.Articles;

namespace WebAPI.IntegrationTests.ArticlesControllerEndpoints;

[Collection("Tests")]
public sealed class DeleteArticleTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
    private readonly CustomWebApplicationFactory _factory = factory;

    [Fact]
    public async Task ArticleCanBeDeleted()
    {
        // Arrange
        var gqlClient = _factory.AuthorizeAs(UserTypes.Admin).CreateClient().ToGqlClient();
        
        const string articleId = "article-id";
        var input = Requests.Article.ArticleInput with { ArticleId = articleId };
        await gqlClient.CreateArticle(input);

        // Act
        var deleteArticleResponse = await gqlClient.DeleteArticle(articleId);
        var exception = await Should.ThrowAsync<GraphQLHttpRequestException>(async () =>
        {
            await gqlClient.GetArticle(articleId);
        });

        // Assert
        deleteArticleResponse.ShouldBe(articleId);
        exception.Content!.ShouldContain(Article.Errors.NotFound.Code);
    }

    [Fact]
    public async Task ArticleCannotBeDeletedIfItDoesNotExist()
    {
        // Arrange
        var gqlClient = _factory.AuthorizeAs(UserTypes.Admin).CreateClient().ToGqlClient();
        const string articleId = "article-id";

        // Act
        var exception = await Should.ThrowAsync<GraphQLHttpRequestException>(async () =>
        {
            await gqlClient.DeleteArticle(articleId);
        });

        // Assert
        exception.Content!.ShouldContain(Article.Errors.NotFound.Code);
    }

    [Fact]
    public async Task OnlyAuthorizedUserCanDeleteArticle()
    {
        // Arrange
        var gqlClient = _factory.CreateClient().ToGqlClient();
        const string articleId = "article-id";

        // Act
        var exception = await Should.ThrowAsync<GraphQLHttpRequestException>(async () =>
        {
            await gqlClient.DeleteArticle(articleId);
        });

        // Assert
        exception.Content!.ShouldContain("AUTH_NOT_AUTHORIZED");
    }

    [Fact]
    public async Task OnlyUserWithCanManageArticlesClaimCanDeleteArticle()
    {
        // Arrange
        var gqlClient = _factory.AuthorizeAs(UserTypes.User).CreateClient().ToGqlClient();
        const string articleId = "article-id";

        // Act
        var exception = await Should.ThrowAsync<GraphQLHttpRequestException>(async () =>
        {
            await gqlClient.DeleteArticle(articleId);
        });

        // Assert
        exception.Content!.ShouldContain("AUTH_NOT_AUTHORIZED");
    }
}
