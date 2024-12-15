namespace Domain.Tests.Users;

public sealed class UserTests
{
    [Fact]
    public void Valid_user_can_be_created()
    {
        // Arrange
        var sut = User.Create(
            email: "example@gmail.com",
            password: "1234");

        // Act
        var result = sut.IsError;

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void User_with_invalid_email_cant_be_created()
    {
        // Arrange
        var sut = User.Create(
            email: "example.com",
            password: "1234");

        // Act
        var result = sut.FirstError;

        // Assert
        result.Should().Be(Domain.Users.Errors.User.InvalidEmail);
    }

    [Fact]
    public void User_with_invalid_password_cant_be_created()
    {
        // Arrange
        var sut = User.Create(
            email: "example@gmail.com",
            password: "123");

        // Act
        var result = sut.FirstError;

        // Assert
        result.Should().Be(Domain.Users.Errors.User.InvalidPassword);
    }
}