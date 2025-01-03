namespace WebAPI.IntegrationTests;

public static class ServiceCollectionExtensions
{
    public static void AdminLogIn(this IServiceCollection services) => services
        .AddAuthentication(defaultScheme: "TestScheme")
        .AddScheme<AuthenticationSchemeOptions, TestAdminAuthHandler>("TestScheme", _ => { });
    
    public static void UserLogIn(this IServiceCollection services) => services
        .AddAuthentication(defaultScheme: "TestScheme")
        .AddScheme<AuthenticationSchemeOptions, TestUserAuthHandler>("TestScheme", _ => { });

    public static void LogOut(this IServiceCollection services) => services
        .RemoveAll<IAuthenticationHandler>();
}