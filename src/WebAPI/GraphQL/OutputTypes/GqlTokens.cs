namespace WebAPI.GraphQL.OutputTypes;

[GraphQLName("tokens")]
public sealed class GqlTokens()
{
    [GraphQLName("accessToken")]
    [GraphQLDescription("Access токен")]
    public string AccessToken { get; init; } = string.Empty;

    [GraphQLName("refreshToken")]
    [GraphQLDescription("Refresh токен")]
    public string RefreshToken { get; init; } = string.Empty;
}
