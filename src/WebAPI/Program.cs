var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks();
builder.Services.AddCors(builder.Configuration);
builder.Services.AddOptions(builder.Configuration);
builder.Services.AddRepositories();
builder.Services.AddGraphQL(builder);
builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        x => x.MigrationsAssembly("Database.Migrator")));

builder.Services.AddJwtBearerAuthentication(builder.Configuration);
builder.Services.AddAuthorizationBuilder()
    .AddPolicy(
        name: nameof(User.CanManageArticles),
        configurePolicy: policyBuilder => policyBuilder.RequireClaim(nameof(User.CanManageArticles), true.ToString()));

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/error");
}

app.UseCors("AllowClient");
app.UseFileServer();
app.UseAuthentication();
app.UseAuthorization();
app.MapGraphQL();
app.MapHealthChecks("/healthz");
await app.RunAsync();

public partial class Program { }
