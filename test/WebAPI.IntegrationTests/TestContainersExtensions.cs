namespace WebAPI.IntegrationTests;

public static class TestContainersExtensions
{
    public static PostgreSqlContainer CreatePostgres()
    {
        var container = new PostgreSqlBuilder()
            .WithImage("postgres:16.3")
            .WithDatabase("inkless")
            .WithUsername("postgres")
            .WithPassword("1234")
            .Build();

        return container;
    }
}
