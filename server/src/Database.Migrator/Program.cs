using System.Reflection;
using Database;
using Database.Migrator;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

var configurationBuilder = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false)
    .AddEnvironmentVariables();
           
var configuration = configurationBuilder.Build();

var serviceCollection = new ServiceCollection();
    
serviceCollection
    .AddOptions<AdminAccountOptions>()
    .Bind(configuration.GetSection(AdminAccountOptions.Section))
    .ValidateDataAnnotations()
    .ValidateOnStart();

var serviceProvider = serviceCollection.BuildServiceProvider();

var adminAccount = serviceProvider.GetService<IOptions<AdminAccountOptions>>()?.Value 
    ?? throw new NullReferenceException($"{nameof(AdminAccountOptions)} options are required.");

var connectionString = args.FirstOrDefault() ?? configuration.GetConnectionString("DefaultConnection");

var options = new DbContextOptionsBuilder<DatabaseContext>()
    .UseNpgsql(connectionString, x => x.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName)).Options;

try
{
    await using var dbContext = new DatabaseContext(options);
    await dbContext.Database.MigrateAsync();
    
    if (!await dbContext.Users.AnyAsync())
    {
        var admin = User.Create(
            email: adminAccount.Email,
            password: adminAccount.Password,
            canManageArticles: true);

        await dbContext.Users.AddAsync(admin.Value);
        await dbContext.SaveChangesAsync();
    }
    
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Success!");
    Console.ResetColor();
}
catch (Exception e)
{
    Console.WriteLine(e);
    return 1;
}

return 0;