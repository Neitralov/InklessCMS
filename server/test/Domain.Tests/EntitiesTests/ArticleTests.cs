namespace Domain.Tests.EntitiesTests;

public class ArticleTests
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
        var sut = Article.Create(
            articleId: id,
            title: "Some title",
            description: "Some description",
            text: "Some text",
            isPublished: true);

        var result = sut.IsError;

        result.Should().BeFalse();
    }
    
    // TODO: Отдельно сделать тесты для Title и для Description
    [Theory]
    [InlineData("abc", "")]
    [InlineData("Some title", "Some description")]
    [InlineData("abcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefgh", "abcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefgh")]
    [InlineData(" abcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefgh ", " abcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefgh ")]
    public void Valid_article_can_be_created(string title, string desciption)
    {
        var sut = Article.Create(
            articleId: "some-id",
            title: title,
            description: desciption,
            text: "Some text",
            isPublished: true);

        var result = sut.IsError;

        result.Should().BeFalse();
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
        var sut = Article.Create(
            articleId: id,
            title: "Some title",
            description: "Some description",
            text: "Some text",
            isPublished: true);

        var result = sut.FirstError;

        result.Should().Be(Errors.Article.InvalidId);
    }
    
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("ab")]
    [InlineData(" a ")]
    [InlineData("abcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefgh_")]
    public void Article_with_invalid_id_length_cant_be_created(string id)
    {
        var sut = Article.Create(
            articleId: id,
            title: "Some title",
            description: "Some description",
            text: "Some text",
            isPublished: true);

        var result = sut.FirstError;

        result.Should().Be(Errors.Article.InvalidIdLength);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("ab")]
    [InlineData(" a ")]
    [InlineData("abcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefgh_")]
    public void Article_with_invalid_title_cant_be_created(string title)
    {
        var sut = Article.Create(
            articleId: "Some-id",
            title: title,
            description: "Some description",
            text: "Some text",
            isPublished: true);

        var result = sut.FirstError;

        result.Should().Be(Errors.Article.InvalidTitleLength);
    }

    [Theory]
    [InlineData("abcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefghabcdefgh_")]
    public void Article_with_invalid_description_cant_be_created(string description)
    {
        var sut = Article.Create(
            articleId: "Some-id",
            title: "Some title",
            description: description,
            text: "Some text",
            isPublished: true);

        var result = sut.FirstError;

        result.Should().Be(Errors.Article.InvalidDescriptionLength);
    }
}
