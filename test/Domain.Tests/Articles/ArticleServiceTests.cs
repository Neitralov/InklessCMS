namespace Domain.Tests.Articles;

public sealed class ArticleServiceTests
{
    private readonly Mock<IArticleRepository> _articleRepositoryMock = new();

    private const string ArticleId = "article-id";
    private readonly Article _validArticle = Article.Create(
        articleId: ArticleId,
        title: "Title",
        description: "Description",
        text: "Text",
        isPublished: true).Value;

    private readonly Article _pinnedValidArticle = Article.Create(
        articleId: ArticleId,
        title: "Title",
        description: "Description",
        text: "Text",
        isPublished: true,
        isPinned: true).Value;

    [Fact]
    public async Task Article_can_be_saved()
    {
        // Arrange
        var sut = new ArticleService(_articleRepositoryMock.Object);

        // Act
        var result = await sut.AddArticle(_validArticle);

        // Assert
        result.Value.Should().Be(Result.Created);
        _articleRepositoryMock.Verify(repository => repository.AddArticle(It.IsAny<Article>()), Times.Once);
        _articleRepositoryMock.Verify(repository => repository.SaveChanges(), Times.Once);
    }

    [Fact]
    public async Task Article_will_be_received_if_it_exists()
    {
        // Arrange
        _articleRepositoryMock
            .Setup(repository => repository.FindArticleById(It.IsAny<string>()).Result)
            .Returns(_validArticle);

        var sut = new ArticleService(_articleRepositoryMock.Object);

        // Act
        var result = await sut.GetArticle(ArticleId);

        // Assert
        result.Value.Should().Be(_validArticle);
    }

    [Fact]
    public async Task Article_wont_be_received_if_it_does_not_exists()
    {
        // Arrange
        _articleRepositoryMock
            .Setup(repository => repository.FindArticleById(It.IsAny<string>()).Result)
            .Returns(Article.Errors.NotFound);

        var sut = new ArticleService(_articleRepositoryMock.Object);

        // Act
        var result = await sut.GetArticle(ArticleId);

        // Assert
        result.FirstError.Should().Be(Article.Errors.NotFound);
    }

    [Fact]
    public async Task Published_articles_will_be_received()
    {
        // Arrange
        _articleRepositoryMock
            .Setup(repository =>
                repository.GetPublishedArticles(It.IsAny<PageOptions>(), It.IsAny<CancellationToken>()).Result)
            .Returns(new PagedList<Article>([], 0));

        var sut = new ArticleService(_articleRepositoryMock.Object);

        // Act
        var result = await sut.GetPublishedArticles(new PageOptions(), CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Articles_will_be_received()
    {
        // Arrange
        _articleRepositoryMock
            .Setup(repository =>
                repository.GetArticles(It.IsAny<PageOptions>(), It.IsAny<CancellationToken>()).Result)
            .Returns(new PagedList<Article>([], 0));

        var sut = new ArticleService(_articleRepositoryMock.Object);

        // Act
        var result = await sut.GetArticles(new PageOptions(), CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Article_will_be_updated_if_it_exists()
    {
        // Arrange
        _articleRepositoryMock
            .Setup(repository => repository.FindArticleById(It.IsAny<string>()).Result)
            .Returns(_validArticle);

        var sut = new ArticleService(_articleRepositoryMock.Object);

        // Act
        var result = await sut.UpdateArticle(_validArticle);

        // Assert
        result.Value.Should().Be(Result.Updated);
        _articleRepositoryMock.Verify(repository => repository.SaveChanges(), Times.Once);
    }

    [Fact]
    public async Task Article_wont_be_updated_if_it_does_not_exists()
    {
        // Arrange
        _articleRepositoryMock
            .Setup(repository => repository.FindArticleById(It.IsAny<string>()).Result)
            .Returns(Article.Errors.NotFound);

        var sut = new ArticleService(_articleRepositoryMock.Object);

        // Act
        var result = await sut.UpdateArticle(_validArticle);

        // Assert
        result.FirstError.Should().Be(Article.Errors.NotFound);
        _articleRepositoryMock.Verify(repository => repository.SaveChanges(), Times.Never);
    }

    [Fact]
    public async Task Article_will_be_pinned_if_it_is_unpinned()
    {
        // Arrange
        _articleRepositoryMock
            .Setup(repository => repository.FindArticleById(It.IsAny<string>()).Result)
            .Returns(_validArticle);

        var sut = new ArticleService(_articleRepositoryMock.Object);

        // Act
        var result = await sut.ChangePinState(ArticleId);

        // Assert
        result.Value.Should().Be(Result.Updated);
        _validArticle.IsPinned.Should().BeTrue();
        _articleRepositoryMock.Verify(repository => repository.SaveChanges(), Times.Once);
    }

    [Fact]
    public async Task Article_will_be_unpinned_if_it_is_pinned()
    {
        // Arrange
        _articleRepositoryMock
            .Setup(repository => repository.FindArticleById(It.IsAny<string>()).Result)
            .Returns(_pinnedValidArticle);

        var sut = new ArticleService(_articleRepositoryMock.Object);

        // Act
        var result = await sut.ChangePinState(ArticleId);

        // Assert
        result.Value.Should().Be(Result.Updated);
        _pinnedValidArticle.IsPinned.Should().BeFalse();
        _articleRepositoryMock.Verify(repository => repository.SaveChanges(), Times.Once);
    }

    [Fact]
    public async Task Article_wont_change_is_pinned_if_it_does_not_exists()
    {
        // Arrange
        _articleRepositoryMock
            .Setup(repository => repository.FindArticleById(It.IsAny<string>()).Result)
            .Returns(Article.Errors.NotFound);

        var sut = new ArticleService(_articleRepositoryMock.Object);

        // Act
        var result = await sut.ChangePinState(ArticleId);

        // Assert
        result.FirstError.Should().Be(Article.Errors.NotFound);
        _articleRepositoryMock.Verify(repository => repository.SaveChanges(), Times.Never);
    }

    [Fact]
    public async Task Views_counter_of_article_will_be_increased_if_article_exists()
    {
        // Arrange
        _articleRepositoryMock
            .Setup(repository => repository.FindArticleById(It.IsAny<string>()).Result)
            .Returns(_pinnedValidArticle);

        var sut = new ArticleService(_articleRepositoryMock.Object);

        // Act
        var result = await sut.IncreaseViewsCounter(ArticleId);

        // Assert
        result.Value.Should().Be(Result.Updated);
        _pinnedValidArticle.Views.Should().Be(1);
        _articleRepositoryMock.Verify(repository => repository.SaveChanges(), Times.Once);
    }

    [Fact]
    public async Task Views_counter_of_article_wont_be_increased_if_article_does_not_exists()
    {
        // Arrange
        _articleRepositoryMock
            .Setup(repository => repository.FindArticleById(It.IsAny<string>()).Result)
            .Returns(Article.Errors.NotFound);

        var sut = new ArticleService(_articleRepositoryMock.Object);

        // Act
        var result = await sut.IncreaseViewsCounter(ArticleId);

        // Assert
        result.FirstError.Should().Be(Article.Errors.NotFound);
        _articleRepositoryMock.Verify(repository => repository.SaveChanges(), Times.Never);
    }

    [Fact]
    public async Task Article_will_be_deleted_if_it_exists()
    {
        // Arrange
        _articleRepositoryMock
            .Setup(repository => repository.DeleteArticle(It.IsAny<string>()).Result)
            .Returns(Result.Deleted);

        var sut = new ArticleService(_articleRepositoryMock.Object);

        // Act
        var result = await sut.DeleteArticle(ArticleId);

        // Assert
        result.Value.Should().Be(Result.Deleted);
        _articleRepositoryMock.Verify(repository => repository.SaveChanges(), Times.Once);
    }

    [Fact]
    public async Task Article_wont_be_deleted_if_it_does_not_exists()
    {
        // Arrange
        _articleRepositoryMock
            .Setup(repository => repository.DeleteArticle(It.IsAny<string>()).Result)
            .Returns(Article.Errors.NotFound);

        var sut = new ArticleService(_articleRepositoryMock.Object);

        // Act
        var result = await sut.DeleteArticle(ArticleId);

        // Assert
        result.FirstError.Should().Be(Article.Errors.NotFound);
        _articleRepositoryMock.Verify(repository => repository.SaveChanges(), Times.Never);
    }
}
