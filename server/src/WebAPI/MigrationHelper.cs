namespace WebAPI;

public static class MigrationHelper
{
    public static async Task MigrateDatabaseAsync(this IHost webHost, AdminAccountOptions options)
    {
        await using var scope = webHost.Services.CreateAsyncScope();

        var services = scope.ServiceProvider;

        await using var context = services.GetRequiredService<DatabaseContext>();

        try
        {
            for (var tries = 0; tries < 5; tries++)
            {
                if (await context.Database.CanConnectAsync())
                {
                    await context.Database.MigrateAsync();

                    if (!await context.Users.AnyAsync())
                    {
                        var admin = User.Create(
                            email: options.Email,
                            password: options.Password,
                            canManageArticles: true);

                        await context.Users.AddAsync(admin.Value);
                        await context.SaveChangesAsync();
                    }

                    break;
                }

                await Task.Delay(millisecondsDelay: 2000);
            }
        }
        catch (Exception exception)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(exception, "An error occurred while migrating the database.");
            throw;
        }
    }
}