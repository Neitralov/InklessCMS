var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks();
builder.Services.AddControllers();
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddCors(builder.Configuration);
builder.Services.AddOptions(builder.Configuration);
builder.Services.AddRepositories();
builder.Services.AddServices();

builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddJwtBearerAuthentication(builder.Configuration);
builder.Services.AddAuthorizationBuilder()
    .AddPolicy(
        name: nameof(User.CanManageArticles),
        configurePolicy: policyBuilder => policyBuilder.RequireClaim(nameof(User.CanManageArticles), true.ToString()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwagger();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/error");
}

app.UseCors("AllowInkless");
app.UseCors("AllowClient");
app.UsePathBase("/inkless");
app.UseFileServer();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapFallbackToFile("index.html");
app.MapHealthChecks("/healthz");
await app.RunAsync();

public partial class Program { }