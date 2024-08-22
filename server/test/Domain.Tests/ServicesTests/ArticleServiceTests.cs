using Domain.Utils;

namespace Domain.Tests.ServicesTests;

public class ArticleServiceTests
{
    [Fact]
    public async Task Article_can_be_saved()
    {
        var article = Article.Create(
            articleId: "some-id",
            title: "Some title",
            description: "Some description",
            text: "Some text",
            isPublished: true);

        var articleRepositoryMock = new Mock<IArticleRepository>();
        var sut = new ArticleService(articleRepositoryMock.Object);

        var result = await sut.StoreArticle(article.Value);

        result.Value.Should().Be(Result.Created);
        articleRepositoryMock.Verify(repository => repository.AddArticle(It.IsAny<Article>()), Times.Once);
        articleRepositoryMock.Verify(repository => repository.SaveChanges(), Times.Once);
    }

    [Fact]
    public async Task Article_will_be_received_if_it_exists()
    {
        var article = Article.Create(
            articleId: "some-id",
            title: "Some title",
            description: "Some description",
            text: "Some text",
            isPublished: true);

        var articleRepositoryMock = new Mock<IArticleRepository>();
        articleRepositoryMock.Setup(repository => repository.FindArticleById(It.IsAny<string>()).Result).Returns(article.Value);
        var sut = new ArticleService(articleRepositoryMock.Object);

        var result = await sut.GetArticle("some-id");

        result.Value.Should().Be(article.Value);
    }

    [Fact]
    public async Task Article_wont_be_received_if_it_does_not_exists()
    {
        var articleRepositoryMock = new Mock<IArticleRepository>();
        articleRepositoryMock.Setup(repository => repository.FindArticleById(It.IsAny<string>()).Result).Returns(value: null);
        var sut = new ArticleService(articleRepositoryMock.Object);

        var result = await sut.GetArticle("some-id");

        result.FirstError.Should().Be(Errors.Article.NotFound);
    }

    [Fact]
    public async Task Published_articles_will_be_received()
    {
        var articleRepositoryMock = new Mock<IArticleRepository>();
        articleRepositoryMock.Setup(repository => 
            repository
                .GetPublishedArticles(It.IsAny<int>(), It.IsAny<int>()).Result)
                .Returns(new PagedList<Article>([], 0));
        var sut = new ArticleService(articleRepositoryMock.Object);

        var result = await sut.GetPublishedArticles(page: 1, size: 10);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Articles_will_be_received()
    {
        var articleRepositoryMock = new Mock<IArticleRepository>();
        articleRepositoryMock.Setup(repository => repository.GetArticles(It.IsAny<int>(), It.IsAny<int>()).Result).Returns(new PagedList<Article>([], 0));
        var sut = new ArticleService(articleRepositoryMock.Object);

        var result = await sut.GetArticles(page: 1, size: 10);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Article_will_be_updated_if_it_exists()
    {
        var article = Article.Create(
            articleId: "some-id",
            title: "Some title",
            description: "Some description",
            text: "Some text",
            isPublished: true);

        var articleRepositoryMock = new Mock<IArticleRepository>();
        articleRepositoryMock.Setup(repository => repository.FindArticleById(It.IsAny<string>()).Result).Returns(article.Value);
        var sut = new ArticleService(articleRepositoryMock.Object);

        var result = await sut.UpdateArticle(article.Value);

        result.Value.Should().Be(Result.Updated);
        articleRepositoryMock.Verify(repository => repository.SaveChanges(), Times.Once);
    }

    [Fact]
    public async Task Article_wont_be_updated_if_it_does_not_exists()
    {
        var article = Article.Create(
            articleId: "some-id",
            title: "Some title",
            description: "Some description",
            text: "Some text",
            isPublished: true);

        var articleRepositoryMock = new Mock<IArticleRepository>();
        articleRepositoryMock.Setup(repository => repository.FindArticleById(It.IsAny<string>()).Result).Returns(value: null);
        var sut = new ArticleService(articleRepositoryMock.Object);

        var result = await sut.UpdateArticle(article.Value);

        result.FirstError.Should().Be(Errors.Article.NotFound);
        articleRepositoryMock.Verify(repository => repository.SaveChanges(), Times.Never);
    }

    [Fact]
    public async Task Article_will_be_pinned_if_it_is_unpinned()
    {
        var article = Article.Create(
            articleId: "some-id",
            title: "Some title",
            description: "Some description",
            text: "Some text",
            isPublished: true);

        var articleRepositoryMock = new Mock<IArticleRepository>();
        articleRepositoryMock.Setup(repository => repository.FindArticleById(It.IsAny<string>()).Result).Returns(article.Value);
        var sut = new ArticleService(articleRepositoryMock.Object);

        var result = await sut.ChangePinState("some-id");

        result.Value.Should().Be(Result.Updated);
        article.Value.IsPinned.Should().BeTrue();
        articleRepositoryMock.Verify(repository => repository.SaveChanges(), Times.Once);
    }

    [Fact]
    public async Task Article_will_be_unpinned_if_it_is_pinned()
    {
        var article = Article.Create(
            articleId: "some-id",
            title: "Some title",
            description: "Some description",
            text: "Some text",
            isPublished: true,
            isPinned: true);

        var articleRepositoryMock = new Mock<IArticleRepository>();
        articleRepositoryMock.Setup(repository => repository.FindArticleById(It.IsAny<string>()).Result).Returns(article.Value);
        var sut = new ArticleService(articleRepositoryMock.Object);

        var result = await sut.ChangePinState("some-id");

        result.Value.Should().Be(Result.Updated);
        article.Value.IsPinned.Should().BeFalse();
        articleRepositoryMock.Verify(repository => repository.SaveChanges(), Times.Once);
    }

    [Fact]
    public async Task Article_wont_change_is_pinned_if_it_does_not_exists()
    {
        var articleRepositoryMock = new Mock<IArticleRepository>();
        articleRepositoryMock.Setup(repository => repository.FindArticleById(It.IsAny<string>()).Result).Returns(value: null);
        var sut = new ArticleService(articleRepositoryMock.Object);

        var result = await sut.ChangePinState("some-id");

        result.FirstError.Should().Be(Errors.Article.NotFound);
        articleRepositoryMock.Verify(repository => repository.SaveChanges(), Times.Never);
    }

    [Fact]
    public async Task Views_counter_of_article_will_be_increased_if_article_exists()
    {
        var article = Article.Create(
            articleId: "some-id",
            title: "Some title",
            description: "Some description",
            text: "Some text",
            isPublished: true,
            isPinned: true);

        var articleRepositoryMock = new Mock<IArticleRepository>();
        articleRepositoryMock.Setup(repository => repository.FindArticleById(It.IsAny<string>()).Result).Returns(article.Value);
        var sut = new ArticleService(articleRepositoryMock.Object);

        var result = await sut.IncreaseViewsCounter("some-id");

        result.Value.Should().Be(Result.Updated);
        article.Value.Views.Should().Be(1);
        articleRepositoryMock.Verify(repository => repository.SaveChanges(), Times.Once);
    }

    [Fact]
    public async Task Views_counter_of_article_wont_be_increased_if_article_does_not_exists()
    {
        var articleRepositoryMock = new Mock<IArticleRepository>();
        articleRepositoryMock.Setup(repository => repository.FindArticleById(It.IsAny<string>()).Result).Returns(value: null);
        var sut = new ArticleService(articleRepositoryMock.Object);

        var result = await sut.IncreaseViewsCounter("some-id");

        result.FirstError.Should().Be(Errors.Article.NotFound);
        articleRepositoryMock.Verify(repository => repository.SaveChanges(), Times.Never);
    }

    [Fact]
    public async Task Article_will_be_deleted_if_it_exists()
    {
        var articleRepositoryMock = new Mock<IArticleRepository>();
        articleRepositoryMock.Setup(repository => repository.DeleteArticle(It.IsAny<string>()).Result).Returns(true);
        var sut = new ArticleService(articleRepositoryMock.Object);

        var result = await sut.DeleteArticle("some-id");

        result.Value.Should().Be(Result.Deleted);
        articleRepositoryMock.Verify(repository => repository.SaveChanges(), Times.Once);
    }

    [Fact]
    public async Task Article_wont_be_deleted_if_it_does_not_exists()
    {
        var articleRepositoryMock = new Mock<IArticleRepository>();
        articleRepositoryMock.Setup(repository => repository.DeleteArticle(It.IsAny<string>()).Result).Returns(false);
        var sut = new ArticleService(articleRepositoryMock.Object);

        var result = await sut.DeleteArticle("some-id");

        result.FirstError.Should().Be(Errors.Article.NotFound);
        articleRepositoryMock.Verify(repository => repository.SaveChanges(), Times.Never);
    }
}
