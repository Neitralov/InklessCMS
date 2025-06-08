namespace Domain.Tests.Articles;

public sealed class ArticleTests
{
    [Theory]
    [InlineData("abc")]
    [InlineData("AbC")]
    [InlineData("1bc2")]
    [InlineData("-bc-")]
    [InlineData("123")]
    [InlineData("---")]
    [InlineData("aZAz-123456789")]
    [InlineData("some-title")]
    [InlineData("SoMe--TiTlE")]
    [InlineData("How-to-write-stupid-ids-part-10")]
    [InlineData("20-Yet-Another--id")]
    public void Article_with_valid_id_can_be_created(string id)
    {
        // Arrange
        var sut = Article.Create(
            articleId: id,
            title: "Some title",
            description: "Some description",
            text: "Some text",
            isPublished: true);

        // Act
        var result = sut.IsError;

        // Assert
        result.ShouldBeFalse();
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("Some title")]
    [InlineData("abcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefgh")]
    [InlineData(" abcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefgh ")]
    public void Article_with_valid_title_can_be_created(string title)
    {
        // Arrange
        var sut = Article.Create(
            articleId: "some-id",
            title: title,
            description: "Some description",
            text: "Some text",
            isPublished: true);

        // Act
        var result = sut.IsError;

        // Assert
        result.ShouldBeFalse();
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("Some description")]
    [InlineData("abcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefgh")]
    [InlineData(" abcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefgh ")]
    public void Article_with_valid_description_can_be_created(string desciption)
    {
        // Arrange
        var sut = Article.Create(
            articleId: "some-id",
            title: "Some title",
            description: desciption,
            text: "Some text",
            isPublished: true);

        // Act
        var result = sut.IsError;

        // Assert
        result.ShouldBeFalse();
    }

    [Theory]
    [InlineData("_aa")]
    [InlineData("aa_")]
    [InlineData("Some-inv@lid-id")]
    [InlineData("Some_invalid_id")]
    [InlineData("Some invalid id")]
    [InlineData("Some$invalid$id")]
    [InlineData("`~!@#$%^&*()_+|\\/[]{};':\"")]
    public void Article_with_invalid_id_cant_be_created(string id)
    {
        // Arrange
        var sut = Article.Create(
            articleId: id,
            title: "Some title",
            description: "Some description",
            text: "Some text",
            isPublished: true);

        // Act
        var result = sut.FirstError;

        // Assert
        result.ShouldBe(Article.Errors.InvalidId);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("ab")]
    [InlineData(" a ")]
    [InlineData("abcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefgh_")]
    public void Article_with_invalid_id_length_cant_be_created(string id)
    {
        // Arrange
        var sut = Article.Create(
            articleId: id,
            title: "Some title",
            description: "Some description",
            text: "Some text",
            isPublished: true);

        // Act
        var result = sut.FirstError;

        // Assert
        result.ShouldBe(Article.Errors.InvalidIdLength);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("ab")]
    [InlineData(" a ")]
    [InlineData("abcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefgh_")]
    public void Article_with_invalid_title_cant_be_created(string title)
    {
        // Arrange
        var sut = Article.Create(
            articleId: "Some-id",
            title: title,
            description: "Some description",
            text: "Some text",
            isPublished: true);

        // Act
        var result = sut.FirstError;

        // Assert
        result.ShouldBe(Article.Errors.InvalidTitleLength);
    }

    [Theory]
    [InlineData("abcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefgh_")]
    public void Article_with_invalid_description_cant_be_created(string description)
    {
        // Arrange
        var sut = Article.Create(
            articleId: "Some-id",
            title: "Some title",
            description: description,
            text: "Some text",
            isPublished: true);

        // Act
        var result = sut.FirstError;

        // Assert
        result.ShouldBe(Article.Errors.InvalidDescriptionLength);
    }
}
