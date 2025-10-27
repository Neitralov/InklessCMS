namespace WebAPI.IntegrationTests.ArticlesControllerEndpoints;

[Collection("Tests")]
public sealed class CreateArticleTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
    private readonly CustomWebApplicationFactory _factory = factory;

    [Fact]
    public async Task ArticleCanBeCreated()
    {
        // Arrange
        var gqlClient = _factory.AuthorizeAs(UserTypes.Admin).CreateClient().ToGqlClient();
        const string articleId = "article-id";

        // Act
        var gqlResponse = await gqlClient.CreateArticle(Inputs.Article.ArticleInput with { ArticleId = articleId });

        // Assert
        gqlResponse.ArticleId.ShouldBe(articleId);
    }

    [Fact]
    public async Task InvalidArticleCannotBeCreated()
    {
        // Arrange
        var gqlClient = _factory.AuthorizeAs(UserTypes.Admin).CreateClient().ToGqlClient();
        const string invalidArticleId = "Inv@lid-Id";

        // Act
        var exception = await Should.ThrowAsync<GraphQLException>(async () =>
        {
            await gqlClient.CreateArticle(Inputs.Article.ArticleInput with { ArticleId = invalidArticleId });
        });
        
        // Assert
        exception.Message!.ShouldContain(Article.Errors.InvalidId.Code);
    }

    [Fact]
    public async Task OnlyAuthorizedUserCanCreateArticle()
    {
        // Arrange
        var gqlClient = _factory.CreateClient().ToGqlClient();
        
        // Act
        var exception = await Should.ThrowAsync<GraphQLException>(async () =>
        {
            await gqlClient.CreateArticle(Inputs.Article.ArticleInput);
        });
        
        // Assert
        exception.Message!.ShouldContain("The current user is not authorized to access this resource.");
    }

    [Fact]
    public async Task OnlyUserWithCanManageArticlesClaimCanCreateArticle()
    {
        // Arrange
        var gqlClient = _factory.AuthorizeAs(UserTypes.User).CreateClient().ToGqlClient();

        // Act
        var exception = await Should.ThrowAsync<GraphQLException>(async () =>
        {
            await gqlClient.CreateArticle(Inputs.Article.ArticleInput);
        });
        
        // Assert
        exception.Message!.ShouldContain("The current user is not authorized to access this resource.");
    }
}
