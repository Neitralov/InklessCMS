var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHealthChecks();

builder.Services
    .AddOptions<JwtOptions>()
    .Bind(builder.Configuration.GetSection(JwtOptions.Section))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services
    .AddOptions<AdminAccountOptions>()
    .Bind(builder.Configuration.GetSection(AdminAccountOptions.Section))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddTransient<IArticleRepository, ArticleRepository>();
builder.Services.AddTransient<ArticleService>();
builder.Services.AddTransient<IAuthService, AuthService>();
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<UserService>();
builder.Services.AddTransient<ICollectionRepository, CollectionRepository>();
builder.Services.AddTransient<CollectionService>();

builder.Services.AddCors(
    options => options.AddPolicy("AllowClient", policy =>
        policy
            .WithOrigins(builder.Configuration["ClientUrl"] ?? throw new NullReferenceException("config variable \"ClientUrl\" is not defined"))
            .AllowAnyHeader()
            .AllowAnyMethod()
            .WithExposedHeaders("X-Total-Count")));

builder.Services.AddCors(
    options => options.AddPolicy("AllowInkless", policy =>
        policy
            .WithOrigins(builder.Configuration["DashboardUrl"] ?? throw new NullReferenceException("config variable \"DashboardUrl\" is not defined"))
            .AllowAnyHeader()
            .AllowAnyMethod()
            .WithExposedHeaders("X-Total-Count")));

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8
                .GetBytes(builder.Configuration["Jwt:SecretKey"] ?? throw new NullReferenceException("config variable \"SecretKey\" is not defined"))),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy(
        name: nameof(User.CanManageArticles),
        configurePolicy: policyBuilder => policyBuilder.RequireClaim(nameof(User.CanManageArticles), true.ToString()));

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Inkless API", Version = "0.1" });

    var xmlDocPaths = Directory.GetFiles(AppContext.BaseDirectory, "*.xml", SearchOption.TopDirectoryOnly).ToList();
    xmlDocPaths.ForEach(xmlDocPath => options.IncludeXmlComments(xmlDocPath));

    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter JWT token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = JwtBearerDefaults.AuthenticationScheme
    };

    options.AddSecurityDefinition(jwtSecurityScheme.Scheme, jwtSecurityScheme);
    options.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();
    options.OperationFilter<SecurityRequirementsOperationFilter>(true, JwtBearerDefaults.AuthenticationScheme);
});

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
await app.MigrateDatabaseAsync(app.Services.GetRequiredService<IOptionsMonitor<AdminAccountOptions>>().CurrentValue);
app.MapHealthChecks("/healthz");
await app.RunAsync();
