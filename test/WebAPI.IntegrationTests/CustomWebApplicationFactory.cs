namespace WebAPI.IntegrationTests;

public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private PostgreSqlContainer _postgresContainer { get; } = TestContainersExtensions.CreatePostgres();

    private Respawner _respawner = null!;
    private DbConnection _connection = null!;

    public async Task InitializeAsync()
    {
        await _postgresContainer.StartAsync();

        var connectionString = _postgresContainer.GetConnectionString();
        var args = new[] { connectionString };
        await Database.Migrator.Program.Main(args);

        var database = Services.CreateScope().ServiceProvider.GetRequiredService<DatabaseContext>();
        _connection = database.Database.GetDbConnection();
        await _connection.OpenAsync();

        _respawner = await Respawner.CreateAsync(_connection, new RespawnerOptions
        {
            TablesToIgnore = ["Users"],
            SchemasToInclude = ["public"],
            DbAdapter = DbAdapter.Postgres
        });
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                service => service.ServiceType == typeof(DbContextOptions<DatabaseContext>));

            if (descriptor != null)
                services.Remove(descriptor);

            services.AddDbContext<DatabaseContext>(options =>
            {
                options.UseNpgsql(_postgresContainer.GetConnectionString());
            });
        });
    }

    public new async Task DisposeAsync()
    {
        await _postgresContainer.DisposeAsync();
        await _connection.CloseAsync();
    }

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
