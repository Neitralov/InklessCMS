namespace WebAPI.GraphQL.InputTypes;

[GraphQLName("refreshTokenInput")]
public sealed record GqlRefreshTokenInput
{
    [GraphQLName("expiredAccessToken")]
    [GraphQLDescription("admin@example.ru")]
    public string ExpiredAccessToken { get; init; } = string.Empty;

    [GraphQLName("refreshToken")]
    [GraphQLDescription("admin")]
    public string RefreshToken { get; init; } = string.Empty;
}
