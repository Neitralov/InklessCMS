using Domain.Articles;

namespace WebAPI.IntegrationTests.ArticlesControllerEndpoints;

[Collection("Tests")]
public sealed class ChangePinStateTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
    private readonly CustomWebApplicationFactory _factory = factory;

    [Fact]
    public async Task PinStateCanBeChanged()
    {
        // Arrange
        var gqlClient = _factory.AuthorizeAs(UserTypes.Admin).CreateClient().ToGqlClient();

        const string firstArticleId = "article-1";
        await gqlClient.CreateArticle(Inputs.Article.ArticleInput with
        {
            ArticleId = firstArticleId,
            IsPinned = false
        });

        const string secondArticleId = "article-2";
        await gqlClient.CreateArticle(Inputs.Article.ArticleInput with
        {
            ArticleId = secondArticleId,
            IsPinned = true
        });

        // Act
        var gqlResponse1 = await gqlClient.ChangePinState(firstArticleId);
        var gqlResponse2 = await gqlClient.ChangePinState(secondArticleId);

        // Assert
        gqlResponse1.IsPinned.ShouldBeTrue();
        gqlResponse2.IsPinned.ShouldBeFalse();
    }

    [Fact]
    public async Task PinStateCannotBeChangedIfArticleDoesNotExist()
    {
        // Arrange
        var gqlClient = _factory.AuthorizeAs(UserTypes.Admin).CreateClient().ToGqlClient();
        const string articleId = "article-id";

        // Act
        var exception = await Should.ThrowAsync<GraphQLException>(async () =>
        {
            await gqlClient.ChangePinState(articleId);
        });
        
        // Assert
        exception.Message!.ShouldContain(Article.Errors.NotFound.Code);
    }

    [Fact]
    public async Task OnlyAuthorizedUserCanChangePinState()
    {
        // Arrange
        var gqlClient = _factory.CreateClient().ToGqlClient();
        const string articleId = "article-id";

        // Act
        var exception = await Should.ThrowAsync<GraphQLException>(async () =>
        {
            await gqlClient.ChangePinState(articleId);
        });
        
        // Assert
        exception.Message!.ShouldContain("The current user is not authorized to access this resource.");
    }

    [Fact]
    public async Task OnlyUserWithCanManageArticlesClaimCanChangePinState()
    {
        // Arrange
        var gqlClient = _factory.AuthorizeAs(UserTypes.User).CreateClient().ToGqlClient();
        const string articleId = "article-id";

        // Act
        var exception = await Should.ThrowAsync<GraphQLException>(async () =>
        {
            await gqlClient.ChangePinState(articleId);
        });
        
        // Assert
        exception.Message!.ShouldContain("The current user is not authorized to access this resource.");
    }
}
