namespace WebAPI.GraphQL.InputTypes;

[GraphQLName("loginInput")]
public sealed record GqlLoginInput
{
    [GraphQLName("email")]
    [GraphQLDescription("admin@example.ru")]
    public string Email { get; init; } = string.Empty;

    [GraphQLName("password")]
    [GraphQLDescription("admin")]
    public string Password { get; init; } = string.Empty;
}
