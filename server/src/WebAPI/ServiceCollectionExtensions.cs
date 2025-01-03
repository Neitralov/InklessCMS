namespace WebAPI;

public static class ServiceCollectionExtensions
{
    public static void AddRepositories(this IServiceCollection services) =>
        services
            .AddTransient<IUserRepository, UserRepository>()
            .AddTransient<IArticleRepository, ArticleRepository>()
            .AddTransient<ICollectionRepository, CollectionRepository>();
    
    public static void AddServices(this IServiceCollection services) =>
        services
            .AddTransient<IAuthService, AuthService>()
            .AddTransient<UserService>()
            .AddTransient<ArticleService>()
            .AddTransient<CollectionService>();

    public static void AddOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.Section))
            .ValidateDataAnnotations()
            .ValidateOnStart();
    }

    public static void AddCors(this IServiceCollection services, IConfiguration configuration) => services
        .AddCors(options => options.AddPolicy("AllowClient",
            policy => policy
                .WithOrigins(configuration["ClientUrl"] ??
                    throw new NullReferenceException("config variable \"ClientUrl\" is not defined"))
                .AllowAnyHeader()
                .AllowAnyMethod()
                .WithExposedHeaders("X-Total-Count")))
        .AddCors(options => options.AddPolicy("AllowInkless", 
            policy => policy
                .WithOrigins(configuration["DashboardUrl"] ??
                    throw new NullReferenceException("config variable \"DashboardUrl\" is not defined"))
                .AllowAnyHeader()
                .AllowAnyMethod()
                .WithExposedHeaders("X-Total-Count")));

    public static void AddSwagger(this IServiceCollection services) =>
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "Inkless API", Version = "0.3" });

            var xmlDocPaths = 
                Directory.GetFiles(AppContext.BaseDirectory, "*.xml", SearchOption.TopDirectoryOnly).ToList();
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
    
    public static void AddJwtBearerAuthentication(this IServiceCollection services, IConfiguration configuration) =>
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8
                        .GetBytes(configuration["Jwt:SecretKey"] ??
                            throw new NullReferenceException("config variable \"SecretKey\" is not defined"))),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });
}