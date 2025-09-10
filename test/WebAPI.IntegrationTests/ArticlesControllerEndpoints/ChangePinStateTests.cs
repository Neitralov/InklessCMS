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
        await gqlClient.CreateArticle(Requests.Article.ArticleInput with
        {
            ArticleId = firstArticleId,
            IsPinned = false
        });

        const string secondArticleId = "article-2";
        await gqlClient.CreateArticle(Requests.Article.ArticleInput with
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
        var exception = await Should.ThrowAsync<GraphQLHttpRequestException>(async () =>
        {
            await gqlClient.ChangePinState(articleId);
        });
        
        // Assert
        exception.Content!.ShouldContain(Article.Errors.NotFound.Code);
    }

    [Fact]
    public async Task OnlyAuthorizedUserCanChangePinState()
    {
        // Arrange
        var gqlClient = _factory.CreateClient().ToGqlClient();
        const string articleId = "article-id";

        // Act
        var exception = await Should.ThrowAsync<GraphQLHttpRequestException>(async () =>
        {
            await gqlClient.ChangePinState(articleId);
        });
        
        // Assert
        exception.Content!.ShouldContain("AUTH_NOT_AUTHORIZED");
    }

    [Fact]
    public async Task OnlyUserWithCanManageArticlesClaimCanChangePinState()
    {
        // Arrange
        var gqlClient = _factory.AuthorizeAs(UserTypes.User).CreateClient().ToGqlClient();
        const string articleId = "article-id";

        // Act
        var exception = await Should.ThrowAsync<GraphQLHttpRequestException>(async () =>
        {
            await gqlClient.ChangePinState(articleId);
        });
        
        // Assert
        exception.Content!.ShouldContain("AUTH_NOT_AUTHORIZED");
    }
}
