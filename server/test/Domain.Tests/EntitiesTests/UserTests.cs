namespace Domain.Tests.EntitiesTests;

public class UserTests
{
    [Fact]
    public void Valid_user_can_be_created()
    {
        var sut = User.Create(
            email: "example@gmail.com",
            password: "1234");

        var result = sut.IsError;

        result.Should().BeFalse();
    }

    [Fact]
    public void User_with_invalid_email_cant_be_created()
    {
        var sut = User.Create(
            email: "example.com",
            password: "1234");

        var result = sut.FirstError;

        result.Should().Be(Errors.User.InvalidEmail);
    }
    
    [Fact]
    public void User_with_invalid_password_cant_be_created()
    {
        var sut = User.Create(
            email: "example@gmail.com",
            password: "123");

        var result = sut.FirstError;

        result.Should().Be(Errors.User.InvalidPassword);
    }
}