namespace Domain.Tests.Collections;

public sealed class CollectionServiceTests
{
    private readonly Mock<IArticleRepository> _articleRepositoryMock = new();
    private readonly Mock<ICollectionRepository> _collectionRepositoryMock = new();

    private const string ArticleId = "article-id";
    private readonly Article _article = Article.Create(
        articleId: ArticleId,
        title: "Title",
        description: "Description",
        text: "Text",
        isPublished: true).Value;

    private const string CollectionId = "collection-id";
    private readonly Collection _collection = Collection.Create(
        collectionId: CollectionId,
        title: "Title").Value;

    [Fact]
    public async Task Collection_can_be_saved()
    {
        // Arrange
        var sut = new CollectionService(_collectionRepositoryMock.Object, _articleRepositoryMock.Object);

        // Act
        var result = await sut.AddCollection(_collection);

        // Assert
        result.Value.Should().Be(Result.Created);
        _collectionRepositoryMock.Verify(repository => repository.IsCollectionExists(It.IsAny<string>()), Times.Once);
        _collectionRepositoryMock.Verify(repository => repository.AddCollection(It.IsAny<Collection>()), Times.Once);
        _collectionRepositoryMock.Verify(repository => repository.SaveChanges(), Times.Once);
    }

    [Fact]
    public async Task Two_collections_with_same_id_cant_be_saved()
    {
        // Arrange
        _collectionRepositoryMock
            .Setup(repository => repository.IsCollectionExists(It.IsAny<string>()).Result)
            .Returns(true);

        var sut = new CollectionService(_collectionRepositoryMock.Object, _articleRepositoryMock.Object);

        // Act
        var result = await sut.AddCollection(_collection);

        // Assert
        result.FirstError.Should().Be(Collection.Errors.NonUniqueId);
        _collectionRepositoryMock.Verify(repository => repository.IsCollectionExists(It.IsAny<string>()), Times.Once);
        _collectionRepositoryMock.Verify(repository => repository.AddCollection(It.IsAny<Collection>()), Times.Never);
        _collectionRepositoryMock.Verify(repository => repository.SaveChanges(), Times.Never);
    }

    [Fact]
    public async Task Collections_can_be_received()
    {
        // Arrange
        _collectionRepositoryMock
            .Setup(repository => repository.GetCollections().Result)
            .Returns([]);

        var sut = new CollectionService(_collectionRepositoryMock.Object, _articleRepositoryMock.Object);

        // Act
        var result = await sut.GetCollections();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Collection_title_can_be_updated()
    {
        // Arrange
        const string updatedTitle = "Updated title";
        var updatedColection = Collection.Create(
            collectionId: CollectionId,
            title: updatedTitle
        ).Value;

        _collectionRepositoryMock
            .Setup(repository => repository.FindCollectionById(It.IsAny<string>()).Result)
            .Returns(_collection);

        var sut = new CollectionService(_collectionRepositoryMock.Object, _articleRepositoryMock.Object);

        // Act
        var result = await sut.UpdateCollection(updatedColection);

        // Assert
        result.Value.Should().Be(Result.Updated);
        _collection.Title.Should().Be(updatedTitle);
        _collectionRepositoryMock.Verify(repository => repository.SaveChanges(), Times.Once);
    }

    [Fact]
    public async Task Collection_title_cant_be_updated_if_it_does_not_exists()
    {
        // Arrange
        const string updatedTitle = "Updated title";
        var updatedColection = Collection.Create(
            collectionId: CollectionId,
            title: updatedTitle
        ).Value;

        _collectionRepositoryMock
            .Setup(repository => repository.FindCollectionById(It.IsAny<string>()).Result)
            .Returns(Collection.Errors.NotFound);

        var sut = new CollectionService(_collectionRepositoryMock.Object, _articleRepositoryMock.Object);

        // Act
        var result = await sut.UpdateCollection(updatedColection);

        // Assert
        result.FirstError.Should().Be(Collection.Errors.NotFound);
        _collectionRepositoryMock.Verify(repository => repository.SaveChanges(), Times.Never);
    }

    [Fact]
    public async Task Article_can_be_stored_into_collection()
    {
        // Arrange
        _articleRepositoryMock
            .Setup(repository => repository.FindArticleById(It.IsAny<string>()).Result)
            .Returns(_article);

        _collectionRepositoryMock
            .Setup(repository => repository.FindCollectionById(It.IsAny<string>()).Result)
            .Returns(_collection);

        var sut = new CollectionService(_collectionRepositoryMock.Object, _articleRepositoryMock.Object);

        // Act
        var result = await sut.AddArticleToCollection(CollectionId, ArticleId);

        // Assert
        result.Value.Should().Be(Result.Success);
        _collection.Articles.Contains(_article).Should().BeTrue();
        _collectionRepositoryMock.Verify(repository => repository.SaveChanges(), Times.Once);
    }

    [Fact]
    public async Task Article_cant_be_stored_into_collection_twice()
    {
        // Arrange
        _collection.AddArticle(_article);

        _articleRepositoryMock
            .Setup(repository => repository.FindArticleById(It.IsAny<string>()).Result)
            .Returns(_article);

        _collectionRepositoryMock
            .Setup(repository => repository.FindCollectionById(It.IsAny<string>()).Result)
            .Returns(_collection);

        var sut = new CollectionService(_collectionRepositoryMock.Object, _articleRepositoryMock.Object);

        // Act
        var result = await sut.AddArticleToCollection(CollectionId, ArticleId);

        // Assert
        result.FirstError.Should().Be(Collection.Errors.ArticleAlreadyAdded);
        _collectionRepositoryMock.Verify(repository => repository.SaveChanges(), Times.Never);
    }

    [Fact]
    public async Task Article_cant_be_stored_into_non_existent_collection()
    {
        // Arrange
        _articleRepositoryMock
            .Setup(repository => repository.FindArticleById(It.IsAny<string>()).Result)
            .Returns(_article);

        _collectionRepositoryMock
            .Setup(repository => repository.FindCollectionById(It.IsAny<string>()).Result)
            .Returns(Collection.Errors.NotFound);

        var sut = new CollectionService(_collectionRepositoryMock.Object, _articleRepositoryMock.Object);

        // Act
        var result = await sut.AddArticleToCollection(CollectionId, ArticleId);

        // Assert
        result.FirstError.Should().Be(Collection.Errors.NotFound);
        _collectionRepositoryMock.Verify(repository => repository.SaveChanges(), Times.Never);
    }

    [Fact]
    public async Task Non_existent_article_cant_be_stored_into_collection()
    {
        // Arrange
        _articleRepositoryMock
            .Setup(repository => repository.FindArticleById(It.IsAny<string>()).Result)
            .Returns(Article.Errors.NotFound);

        _collectionRepositoryMock
            .Setup(repository => repository.FindCollectionById(It.IsAny<string>()).Result)
            .Returns(_collection);

        var sut = new CollectionService(_collectionRepositoryMock.Object, _articleRepositoryMock.Object);

        // Act
        var result = await sut.AddArticleToCollection(CollectionId, ArticleId);

        // Assert
        result.FirstError.Should().Be(Article.Errors.NotFound);
        _collectionRepositoryMock.Verify(repository => repository.SaveChanges(), Times.Never);
    }

    [Fact]
    public async Task Collection_can_be_received()
    {
        // Arrange
        _collectionRepositoryMock
            .Setup(repository => repository.FindCollectionById(It.IsAny<string>()).Result)
            .Returns(_collection);

        var sut = new CollectionService(_collectionRepositoryMock.Object, _articleRepositoryMock.Object);

        // Act
        var result = await sut.GetCollection(CollectionId);

        // Assert
        result.Value.Should().Be(_collection);
    }

    [Fact]
    public async Task Collection_cant_be_received_if_it_does_not_exists()
    {
        // Arrange
        _collectionRepositoryMock
            .Setup(repository => repository.FindCollectionById(It.IsAny<string>()).Result)
            .Returns(Collection.Errors.NotFound);

        var sut = new CollectionService(_collectionRepositoryMock.Object, _articleRepositoryMock.Object);

        // Act
        var result = await sut.GetCollection(CollectionId);

        // Assert
        result.FirstError.Should().Be(Collection.Errors.NotFound);
    }

    [Fact]
    public async Task Collection_can_be_deleted()
    {
        // Arrange
        _collectionRepositoryMock
            .Setup(repository => repository.DeleteCollection(It.IsAny<string>()).Result)
            .Returns(Result.Deleted);

        var sut = new CollectionService(_collectionRepositoryMock.Object, _articleRepositoryMock.Object);

        // Act
        var result = await sut.DeleteCollection(CollectionId);

        // Assert
        result.Value.Should().Be(Result.Deleted);
        _collectionRepositoryMock.Verify(repository => repository.SaveChanges(), Times.Once);
    }

    [Fact]
    public async Task Collection_cant_be_deleted_if_it_does_not_exists()
    {
        // Arrange
        _collectionRepositoryMock
            .Setup(repository => repository.DeleteCollection(It.IsAny<string>()).Result)
            .Returns(Collection.Errors.NotFound);

        var sut = new CollectionService(_collectionRepositoryMock.Object, _articleRepositoryMock.Object);

        // Act
        var result = await sut.DeleteCollection(CollectionId);

        // Assert
        result.FirstError.Should().Be(Collection.Errors.NotFound);
        _collectionRepositoryMock.Verify(repository => repository.SaveChanges(), Times.Never);
    }

    [Fact]
    public async Task Article_can_be_deleted_from_collection()
    {
        // Arrange
        _collection.AddArticle(_article);

        _collectionRepositoryMock
            .Setup(repository => repository.FindCollectionById(It.IsAny<string>()).Result)
            .Returns(_collection);

        var sut = new CollectionService(_collectionRepositoryMock.Object, _articleRepositoryMock.Object);

        // Act
        var result = await sut.DeleteArticleFromCollection(CollectionId, ArticleId);

        // Assert
        result.Value.Should().Be(Result.Deleted);
        _collectionRepositoryMock.Verify(repository => repository.SaveChanges(), Times.Once);
    }

    [Fact]
    public async Task Non_existent_article_cant_be_deleted_from_collection()
    {
        // Arrange
        _collectionRepositoryMock
            .Setup(repository => repository.FindCollectionById(It.IsAny<string>()).Result)
            .Returns(_collection);

        var sut = new CollectionService(_collectionRepositoryMock.Object, _articleRepositoryMock.Object);

        // Act
        var result = await sut.DeleteArticleFromCollection(CollectionId, ArticleId);

        // Assert
        result.FirstError.Should().Be(Collection.Errors.ArticleNotFound);
        _collectionRepositoryMock.Verify(repository => repository.SaveChanges(), Times.Never);
    }

    [Fact]
    public async Task Article_cant_be_deleted_from_non_existent_collection()
    {
        // Arrange
        _collectionRepositoryMock
            .Setup(repository => repository.FindCollectionById(It.IsAny<string>()).Result)
            .Returns(Collection.Errors.NotFound);

        var sut = new CollectionService(_collectionRepositoryMock.Object, _articleRepositoryMock.Object);

        // Act
        var result = await sut.DeleteArticleFromCollection(CollectionId, ArticleId);

        // Assert
        result.FirstError.Should().Be(Collection.Errors.NotFound);
        _collectionRepositoryMock.Verify(repository => repository.SaveChanges(), Times.Never);
    }
}
