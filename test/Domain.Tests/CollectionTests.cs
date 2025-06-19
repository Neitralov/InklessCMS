namespace Domain.Tests.Collections;

public sealed class CollectionTests
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
    public void Collection_with_valid_id_can_be_created(string id)
    {
        // Arrange
        var sut = Collection.Create(
            collectionId: id,
            title: "Some title");

        // Act
        var result = sut.IsError;

        // Assert
        result.ShouldBeFalse();
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("Some title")]
    [InlineData("abcdefghabcdefghabcdefghabcdefgh")]
    [InlineData(" abcdefghabcdefghabcdefghabcdefgh ")]
    public void Collection_with_valid_title_can_be_created(string title)
    {
        // Arrange
        var sut = Collection.Create(
            collectionId: "some-id",
            title: title);

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
    public void Collection_with_invalid_id_cant_be_created(string id)
    {
        // Arrange
        var sut = Collection.Create(
            collectionId: id,
            title: "Some title");

        // Act
        var result = sut.FirstError;

        // Assert
        result.ShouldBe(Collection.Errors.InvalidId);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("ab")]
    [InlineData(" a ")]
    [InlineData("abcdefghabcdefghabcdefghabcdefgh_")]
    public void Collection_with_invalid_id_length_cant_be_created(string id)
    {
        // Arrange
        var sut = Collection.Create(
            collectionId: id,
            title: "Some title");

        // Act
        var result = sut.FirstError;

        // Assert
        result.ShouldBe(Collection.Errors.InvalidIdLength);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("ab")]
    [InlineData(" a ")]
    [InlineData("abcdefghabcdefghabcdefghabcdefgh_")]
    public void Collection_with_invalid_title_cant_be_created(string title)
    {
        // Arrange
        var sut = Collection.Create(
            collectionId: "some-id",
            title: title);

        // Act
        var result = sut.FirstError;

        // Assert
        result.ShouldBe(Collection.Errors.InvalidTitleLength);
    }
}
