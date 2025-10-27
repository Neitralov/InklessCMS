namespace WebAPI.IntegrationTests.GraphQL.Mutations.UserMutations;

public static partial class Mutations
{
    public static async Task<GqlTokens> RefreshTokens(this GraphQLHttpClient gqlClient, GqlRefreshTokenInput input)
    {
        var gqlResponse = await gqlClient.SendMutationAsync(
            request: RefreshTokens(input),
            defineResponseType: () => new { userMutations = new { refreshTokens = new GqlTokens() } });
            
        if (gqlResponse.Errors is not null)
            throw new GraphQLException(message: gqlResponse.Errors.First().Message);
        
        return gqlResponse.Data.userMutations.refreshTokens;
    }
    
    private static GraphQLHttpRequest RefreshTokens(GqlRefreshTokenInput input) => new()
    {
        Query =
            $$"""
                mutation RefreshTokens($input: refreshTokenInput!) {
                    userMutations {
                        refreshTokens(input: $input) {
                            accessToken
                            refreshToken
                        }
                    }
                }
              """,
        Variables = new
        {
            input
        }
    };
}
