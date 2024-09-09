namespace Domain.Tests.ServicesTests;

public class CollectionServiceTests
{
    [Fact]
    public async Task Collection_can_be_saved()
    {
        var collection = Collection.Create(
            collectionId: "some-id",
            title: "some-title"
        );

        var articleRepositoryMock = new Mock<IArticleRepository>();
        var collectionRepositoryMock = new Mock<ICollectionRepository>();
        var sut = new CollectionService(collectionRepositoryMock.Object, articleRepositoryMock.Object);

        var result = await sut.StoreCollection(collection.Value);

        result.Value.Should().Be(Result.Created);
        collectionRepositoryMock.Verify(repository => repository.IsCollectionExists(It.IsAny<string>()), Times.Once);
        collectionRepositoryMock.Verify(repository => repository.AddCollection(It.IsAny<Collection>()), Times.Once);
        collectionRepositoryMock.Verify(repository => repository.SaveChanges(), Times.Once);
    }

    [Fact]
    public async Task Two_collections_with_same_id_cant_be_saved()
    {
        var collection = Collection.Create(
            collectionId: "some-id",
            title: "some-title"
        );

        var articleRepositoryMock = new Mock<IArticleRepository>();
        var collectionRepositoryMock = new Mock<ICollectionRepository>();
        collectionRepositoryMock.Setup(repository => repository.IsCollectionExists(It.IsAny<string>()).Result).Returns(true);
        var sut = new CollectionService(collectionRepositoryMock.Object, articleRepositoryMock.Object);

        var result = await sut.StoreCollection(collection.Value);

        result.FirstError.Should().Be(Errors.Collection.NonUniqueId);
        collectionRepositoryMock.Verify(repository => repository.IsCollectionExists(It.IsAny<string>()), Times.Once);
        collectionRepositoryMock.Verify(repository => repository.AddCollection(It.IsAny<Collection>()), Times.Never);
        collectionRepositoryMock.Verify(repository => repository.SaveChanges(), Times.Never);
    }

    [Fact]
    public async Task Collections_can_be_received()
    {
        var articleRepositoryMock = new Mock<IArticleRepository>();
        var collectionRepositoryMock = new Mock<ICollectionRepository>();
        collectionRepositoryMock.Setup(repository => repository.GetCollections().Result).Returns([]);
        var sut = new CollectionService(collectionRepositoryMock.Object, articleRepositoryMock.Object);

        var result = await sut.GetCollections();

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Collection_title_can_be_updated()
    {
        var collection = Collection.Create(
            collectionId: "some-id",
            title: "some-title"
        );

        var updatedColection = Collection.Create(
            collectionId: "some-id",
            title: "some-new-title"
        );

        var articleRepositoryMock = new Mock<IArticleRepository>();
        var collectionRepositoryMock = new Mock<ICollectionRepository>();
        collectionRepositoryMock.Setup(repository => repository.FindCollectionById(It.IsAny<string>()).Result).Returns(collection.Value);
        var sut = new CollectionService(collectionRepositoryMock.Object, articleRepositoryMock.Object);

        var result = await sut.UpdateCollection(updatedColection.Value);

        result.Value.Should().Be(Result.Updated);
        collection.Value.Title.Should().Be("some-new-title");
        collectionRepositoryMock.Verify(repository => repository.SaveChanges(), Times.Once);
    }

    [Fact]
    public async Task Collection_title_cant_be_updated_if_it_does_not_exists()
    {
        var updatedColection = Collection.Create(
            collectionId: "some-id",
            title: "some-new-title"
        );

        var articleRepositoryMock = new Mock<IArticleRepository>();
        var collectionRepositoryMock = new Mock<ICollectionRepository>();
        collectionRepositoryMock.Setup(repository => repository.FindCollectionById(It.IsAny<string>()).Result).Returns(Errors.Collection.NotFound);
        var sut = new CollectionService(collectionRepositoryMock.Object, articleRepositoryMock.Object);

        var result = await sut.UpdateCollection(updatedColection.Value);

        result.FirstError.Should().Be(Errors.Collection.NotFound);
        collectionRepositoryMock.Verify(repository => repository.SaveChanges(), Times.Never);
    }

    [Fact]
    public async Task Article_can_be_stored_into_collection()
    {
        var article = Article.Create(
            articleId: "some-id",
            title: "Some title",
            description: "Some description",
            text: "Some text",
            isPublished: true
        );

        var collection = Collection.Create(
            collectionId: "some-id",
            title: "some-title"
        );

        var articleRepositoryMock = new Mock<IArticleRepository>();
        articleRepositoryMock.Setup(repository => repository.FindArticleById(It.IsAny<string>()).Result).Returns(article.Value);
        var collectionRepositoryMock = new Mock<ICollectionRepository>();
        collectionRepositoryMock.Setup(repository => repository.FindCollectionById(It.IsAny<string>()).Result).Returns(collection.Value);
        var sut = new CollectionService(collectionRepositoryMock.Object, articleRepositoryMock.Object);

        var result = await sut.StoreArticleToCollection("some-id", "some-id");

        result.Value.Should().Be(Result.Success);
        collection.Value.Articles.Contains(article.Value).Should().BeTrue();
        collectionRepositoryMock.Verify(repository => repository.SaveChanges(), Times.Once);
    }

    [Fact]
    public async Task Article_cant_be_stored_into_collection_twice()
    {
        var article = Article.Create(
            articleId: "some-id",
            title: "Some title",
            description: "Some description",
            text: "Some text",
            isPublished: true
        );

        var collection = Collection.Create(
            collectionId: "some-id",
            title: "some-title"
        );

        collection.Value.AddArticle(article.Value);
        var articleRepositoryMock = new Mock<IArticleRepository>();
        articleRepositoryMock.Setup(repository => repository.FindArticleById(It.IsAny<string>()).Result).Returns(article.Value);
        var collectionRepositoryMock = new Mock<ICollectionRepository>();
        collectionRepositoryMock.Setup(repository => repository.FindCollectionById(It.IsAny<string>()).Result).Returns(collection.Value);
        var sut = new CollectionService(collectionRepositoryMock.Object, articleRepositoryMock.Object);

        var result = await sut.StoreArticleToCollection("some-id", "some-id");

        result.FirstError.Should().Be(Errors.Collection.ArticleAlreadyAdded);
        collectionRepositoryMock.Verify(repository => repository.SaveChanges(), Times.Never);
    }

    [Fact]
    public async Task Article_cant_be_stored_into_non_existent_collection()
    {
        var article = Article.Create(
            articleId: "some-id",
            title: "Some title",
            description: "Some description",
            text: "Some text",
            isPublished: true
        );

        var articleRepositoryMock = new Mock<IArticleRepository>();
        articleRepositoryMock.Setup(repository => repository.FindArticleById(It.IsAny<string>()).Result).Returns(article.Value);
        var collectionRepositoryMock = new Mock<ICollectionRepository>();
        collectionRepositoryMock.Setup(repository => repository.FindCollectionById(It.IsAny<string>()).Result).Returns(Errors.Collection.NotFound);
        var sut = new CollectionService(collectionRepositoryMock.Object, articleRepositoryMock.Object);

        var result = await sut.StoreArticleToCollection("some-id", "some-id");

        result.FirstError.Should().Be(Errors.Collection.NotFound);
        collectionRepositoryMock.Verify(repository => repository.SaveChanges(), Times.Never);
    }

    [Fact]
    public async Task Non_existent_article_cant_be_stored_into_collection()
    {
        var collection = Collection.Create(
            collectionId: "some-id",
            title: "some-title"
        );

        var articleRepositoryMock = new Mock<IArticleRepository>();
        articleRepositoryMock.Setup(repository => repository.FindArticleById(It.IsAny<string>()).Result).Returns(Errors.Article.NotFound);
        var collectionRepositoryMock = new Mock<ICollectionRepository>();
        collectionRepositoryMock.Setup(repository => repository.FindCollectionById(It.IsAny<string>()).Result).Returns(collection.Value);
        var sut = new CollectionService(collectionRepositoryMock.Object, articleRepositoryMock.Object);

        var result = await sut.StoreArticleToCollection("some-id", "some-id");

        result.FirstError.Should().Be(Errors.Article.NotFound);
        collectionRepositoryMock.Verify(repository => repository.SaveChanges(), Times.Never);
    }

    [Fact]
    public async Task Collection_can_be_received()
    {
        var collection = Collection.Create(
            collectionId: "some-id",
            title: "some-title"
        );

        var articleRepositoryMock = new Mock<IArticleRepository>();
        var collectionRepositoryMock = new Mock<ICollectionRepository>();
        collectionRepositoryMock.Setup(repository => repository.FindCollectionById(It.IsAny<string>()).Result).Returns(collection.Value);
        var sut = new CollectionService(collectionRepositoryMock.Object, articleRepositoryMock.Object);

        var result = await sut.GetCollection("some-id");

        result.Value.Should().Be(collection.Value);
    }

    [Fact]
    public async Task Collection_cant_be_received_if_it_does_not_exists()
    {
        var articleRepositoryMock = new Mock<IArticleRepository>();
        var collectionRepositoryMock = new Mock<ICollectionRepository>();
        collectionRepositoryMock.Setup(repository => repository.FindCollectionById(It.IsAny<string>()).Result).Returns(Errors.Collection.NotFound);
        var sut = new CollectionService(collectionRepositoryMock.Object, articleRepositoryMock.Object);

        var result = await sut.GetCollection("some-id");

        result.FirstError.Should().Be(Errors.Collection.NotFound);
    }

    [Fact]
    public async Task Collection_can_be_deleted()
    {
        var articleRepositoryMock = new Mock<IArticleRepository>();
        var collectionRepositoryMock = new Mock<ICollectionRepository>();
        collectionRepositoryMock.Setup(repository => repository.DeleteCollection(It.IsAny<string>()).Result).Returns(Result.Deleted);
        var sut = new CollectionService(collectionRepositoryMock.Object, articleRepositoryMock.Object);

        var result = await sut.DeleteCollection("some-id");

        result.Value.Should().Be(Result.Deleted);
        collectionRepositoryMock.Verify(repository => repository.SaveChanges(), Times.Once);
    }

    [Fact]
    public async Task Collection_cant_be_deleted_if_it_does_not_exists()
    {
        var articleRepositoryMock = new Mock<IArticleRepository>();
        var collectionRepositoryMock = new Mock<ICollectionRepository>();
        collectionRepositoryMock.Setup(repository => repository.DeleteCollection(It.IsAny<string>()).Result).Returns(Errors.Collection.NotFound);
        var sut = new CollectionService(collectionRepositoryMock.Object, articleRepositoryMock.Object);

        var result = await sut.DeleteCollection("some-id");

        result.FirstError.Should().Be(Errors.Collection.NotFound);
        collectionRepositoryMock.Verify(repository => repository.SaveChanges(), Times.Never);
    }

    [Fact]
    public async Task Article_can_be_deleted_from_collection()
    {
        var article = Article.Create(
            articleId: "some-id",
            title: "Some title",
            description: "Some description",
            text: "Some text",
            isPublished: true
        );

        var collection = Collection.Create(
            collectionId: "some-id",
            title: "some-title"
        );

        collection.Value.AddArticle(article.Value);
        var articleRepositoryMock = new Mock<IArticleRepository>();
        var collectionRepositoryMock = new Mock<ICollectionRepository>();
        collectionRepositoryMock.Setup(repository => repository.FindCollectionById(It.IsAny<string>()).Result).Returns(collection.Value);
        var sut = new CollectionService(collectionRepositoryMock.Object, articleRepositoryMock.Object);

        var result = await sut.DeleteArticleFromCollection("some-id", "some-id");

        result.Value.Should().Be(Result.Deleted);
        collectionRepositoryMock.Verify(repository => repository.SaveChanges(), Times.Once);
    }

    [Fact]
    public async Task Non_existent_article_cant_be_deleted_from_collection()
    {
        var collection = Collection.Create(
            collectionId: "some-id",
            title: "some-title"
        );

        var articleRepositoryMock = new Mock<IArticleRepository>();
        var collectionRepositoryMock = new Mock<ICollectionRepository>();
        collectionRepositoryMock.Setup(repository => repository.FindCollectionById(It.IsAny<string>()).Result).Returns(collection.Value);
        var sut = new CollectionService(collectionRepositoryMock.Object, articleRepositoryMock.Object);

        var result = await sut.DeleteArticleFromCollection("some-id", "some-id");

        result.FirstError.Should().Be(Errors.Collection.ArticleNotFound);
        collectionRepositoryMock.Verify(repository => repository.SaveChanges(), Times.Never);
    }

    [Fact]
    public async Task Article_cant_be_deleted_from_non_existent_collection()
    {
        var article = Article.Create(
            articleId: "some-id",
            title: "Some title",
            description: "Some description",
            text: "Some text",
            isPublished: true
        );

        var articleRepositoryMock = new Mock<IArticleRepository>();
        var collectionRepositoryMock = new Mock<ICollectionRepository>();
        collectionRepositoryMock.Setup(repository => repository.FindCollectionById(It.IsAny<string>()).Result).Returns(Errors.Collection.NotFound);
        var sut = new CollectionService(collectionRepositoryMock.Object, articleRepositoryMock.Object);

        var result = await sut.DeleteArticleFromCollection("some-id", "some-id");

        result.FirstError.Should().Be(Errors.Collection.NotFound);
        collectionRepositoryMock.Verify(repository => repository.SaveChanges(), Times.Never);
    }
}
