namespace WebAPI.IntegrationTests.GraphQL.Mutations.UserMutations;

public static partial class Mutations
{
    public static async Task<GqlTokens> Login(this GraphQLHttpClient gqlClient, GqlLoginInput input)
    {
        var gqlResponse = await gqlClient.SendMutationAsync(
            request: Login(input),
            defineResponseType: () => new { userMutations = new { login = new GqlTokens() } });
            
        if (gqlResponse.Errors is not null)
            throw new GraphQLException(message: gqlResponse.Errors.First().Message);
        
        return gqlResponse.Data.userMutations.login;
    }
    
    private static GraphQLHttpRequest Login(GqlLoginInput input) => new()
    {
        Query =
            $$"""
                mutation Login($input: loginInput!) {
                    userMutations {
                        login(input: $input) {
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
