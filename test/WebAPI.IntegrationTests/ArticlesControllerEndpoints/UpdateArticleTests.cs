using Domain.Articles;

namespace WebAPI.IntegrationTests.ArticlesControllerEndpoints;

[Collection("Tests")]
public sealed class UpdateArticleTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
    private readonly CustomWebApplicationFactory _factory = factory;

    [Fact]
    public async Task ArticleCanBeUpdated()
    {
        // Arrange
        var gqlClient = _factory.AuthorizeAs(UserTypes.Admin).CreateClient().ToGqlClient();
        const string articleId = "article-id";
        const string defaultTitle = "Default title";
        const string updatedTitle = "Updated title";

        // Act
        var createGqlResponse = await gqlClient.CreateArticle(Requests.Article.ArticleInput with
        {
            ArticleId = articleId, 
            Title = defaultTitle
        });
        
        var updateGqlResponse = await gqlClient.UpdateArticle(Requests.Article.ArticleInput with
        {
            ArticleId = articleId, 
            Title = updatedTitle
        });

        // Assert
        createGqlResponse.Title.ShouldBe(defaultTitle);
        updateGqlResponse.Title.ShouldBe(updatedTitle);
    }

    [Fact]
    public async Task ArticleCannotBeUpdatedIfItDoesNotExist()
    {
        // Arrange
        var gqlClient = _factory.AuthorizeAs(UserTypes.Admin).CreateClient().ToGqlClient();
        const string articleId = "article-id";

        // Act
        var exception = await Should.ThrowAsync<GraphQLException>(async () =>
        {
            await gqlClient.UpdateArticle(Requests.Article.ArticleInput with { ArticleId = articleId });
        });

        // Assert
        exception.Message!.ShouldContain(Article.Errors.NotFound.Code);
    }

    [Fact]
    public async Task ArticleCannotBeUpdatedWithInvalidData()
    {
        // Arrange
        var gqlClient = _factory.AuthorizeAs(UserTypes.Admin).CreateClient().ToGqlClient();
        const string articleId = "article-id";
        const string tooShortTitle = "Aa";

        await gqlClient.CreateArticle(Requests.Article.ArticleInput with { ArticleId = articleId });

        // Act
        var exception = await Should.ThrowAsync<GraphQLException>(async () =>
        {
            await gqlClient.UpdateArticle(Requests.Article.ArticleInput with
            {
                ArticleId = articleId,
                Title = tooShortTitle
            });
        });

        // Assert
        exception.Message!.ShouldContain(Article.Errors.InvalidTitleLength.Code);
    }

    [Fact]
    public async Task OnlyAuthorizedUserCanUpdateArticle()
    {
        // Arrange
        var gqlClient = _factory.CreateClient().ToGqlClient();
        const string articleId = "article-id";

        // Act
        var exception = await Should.ThrowAsync<GraphQLException>(async () =>
        {
            await gqlClient.UpdateArticle(Requests.Article.ArticleInput with { ArticleId = articleId });
        });
        
        // Assert
        exception.Message!.ShouldContain("The current user is not authorized to access this resource.");
    }

    [Fact]
    public async Task OnlyUserWithCanManageArticlesClaimCanUpdateArticle()
    {
        // Arrange
        var gqlClient = _factory.CreateClient().ToGqlClient();
        const string articleId = "article-id";

        // Act
        var exception = await Should.ThrowAsync<GraphQLException>(async () =>
        {
            await gqlClient.UpdateArticle(Requests.Article.ArticleInput with { ArticleId = articleId });
        });
        
        // Assert
        exception.Message!.ShouldContain("The current user is not authorized to access this resource.");
    }
}
