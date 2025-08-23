namespace WebAPI.GraphQL.OutputTypes;

[GraphQLName("tokens")]
public sealed class GqlTokens(AccessToken accessToken, RefreshToken refreshToken)
{
    [GraphQLName("accessToken")]
    [GraphQLDescription("Access токен")]
    public string AccessToken { get; init; } = accessToken.Token;

    [GraphQLName("refreshToken")]
    [GraphQLDescription("Refresh токен")]
    public string RefreshToken { get; init; } = refreshToken.Token;
}
