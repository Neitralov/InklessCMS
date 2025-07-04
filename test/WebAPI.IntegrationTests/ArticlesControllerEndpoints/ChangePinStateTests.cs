namespace WebAPI.IntegrationTests.ArticlesControllerEndpoints;

[Collection("Tests")]
public sealed class ChangePinStateTests(CustomWebApplicationFactory factory) : BaseIntegrationTest(factory)
{
    private readonly CustomWebApplicationFactory _factory = factory;

    [Fact]
    public async Task PinStateCanBeChanged()
    {
        // Arrange
        var customClient = _factory.AuthorizeAs(UserTypes.Admin).CreateClient();

        const string firstArticleId = "article-1";
        await customClient.PostAsJsonAsync(
            requestUri: "/api/articles",
            value: DataGenerator.Article.GetCreateRequest() with { ArticleId = firstArticleId, IsPinned = false });

        const string secondArticleId = "article-2";
        await customClient.PostAsJsonAsync(
            requestUri: "/api/articles",
            value: DataGenerator.Article.GetCreateRequest() with { ArticleId = secondArticleId, IsPinned = true });

        // Act
        var changePinStateResponse1 = await customClient.PatchAsync($"/api/articles/{firstArticleId}/pin", null);
        var getArticleResponse1 = await customClient.GetAsync($"/api/articles/{firstArticleId}");

        var changePinStateResponse2 = await customClient.PatchAsync($"/api/articles/{secondArticleId}/pin", null);
        var getArticleResponse2 = await customClient.GetAsync($"/api/articles/{secondArticleId}");

        // Assert
        changePinStateResponse1.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        (await getArticleResponse1.Content.ReadFromJsonAsync<ArticleResponse>())?.IsPinned.ShouldBeTrue();

        changePinStateResponse2.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        (await getArticleResponse2.Content.ReadFromJsonAsync<ArticleResponse>())?.IsPinned.ShouldBeFalse();
    }

    [Fact]
    public async Task PinStateCannotBeChangedIfArticleDoesNotExist()
    {
        // Arrange
        var customClient = _factory.AuthorizeAs(UserTypes.Admin).CreateClient();
        const string articleId = "article-id";

        // Act
        var response = await customClient.PatchAsync($"/api/articles/{articleId}/pin", null);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task OnlyAuthorizedUserCanChangePinState()
    {
        // Arrange
        var client = _factory.CreateClient();
        const string articleId = "article-id";

        // Act
        var response = await client.PatchAsync($"/api/articles/{articleId}/pin", null);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task OnlyUserWithCanManageArticlesClaimCanChangePinState()
    {
        // Arrange
        var customClient = _factory.AuthorizeAs(UserTypes.User).CreateClient();
        const string articleId = "article-id";

        // Act
        var response = await customClient.PatchAsync($"/api/articles/{articleId}/pin", null);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
}
