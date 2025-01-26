namespace WebAPI.IntegrationTests;

public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private Respawner _respawner = null!;
    private DbConnection _connection = null!;
    
    public async Task InitializeAsync()
    {
        var database = Services.CreateScope().ServiceProvider.GetRequiredService<DatabaseContext>();
        _connection = database.Database.GetDbConnection();
        await _connection.OpenAsync();
        
        _respawner = await Respawner.CreateAsync(_connection, new RespawnerOptions
        {
            TablesToIgnore = [ "Users" ],
            SchemasToInclude = [ "public" ],
            DbAdapter = DbAdapter.Postgres
        });
    }

    public new async Task DisposeAsync() => await _connection.CloseAsync();
    
    public async Task ResetDatabaseAsync() => await _respawner.ResetAsync(_connection);
}

public static class CustomWebApplicationFactoryExtensions
{
    public static WebApplicationFactory<Program> AuthorizeAs(
        this CustomWebApplicationFactory factory,
        UserTypes userType) =>
        userType switch
        {
            UserTypes.Admin => factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AdminLogIn();
                });
            }),
            UserTypes.User => factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.UserLogIn();
                });
            }),
            _ => throw new ArgumentOutOfRangeException(nameof(userType), userType, null)
        };
}