using System.Reflection;
using Database;
using Microsoft.EntityFrameworkCore;

const string defaultConnectionString = "Host=localhost;Port=5432;Database=inkless;Username=postgres;Password=1234";
var connectionString = args.FirstOrDefault() ?? defaultConnectionString;

var options = new DbContextOptionsBuilder<DatabaseContext>()
    .UseNpgsql(connectionString, x => x.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName)).Options;

try
{
    await using var dbContext = new DatabaseContext(options);
    await dbContext.Database.MigrateAsync();
    
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