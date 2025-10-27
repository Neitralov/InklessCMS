namespace WebAPI;

public static class ServiceCollectionExtensions
{
    public static void AddRepositories(this IServiceCollection services)
    {
        services
            .AddTransient<IUserRepository, UserRepository>()
            .AddTransient<IArticleRepository, ArticleRepository>()
            .AddTransient<ICollectionRepository, CollectionRepository>();
    }

    public static void AddOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.Section))
            .ValidateDataAnnotations()
            .ValidateOnStart();
    }

    public static void AddCors(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCors(options => options.AddPolicy("AllowClient",
            policy => policy
                .WithOrigins(configuration["ClientUrl"] ??
                    throw new NullReferenceException("config variable \"ClientUrl\" is not defined"))
                .AllowAnyHeader()
                .AllowAnyMethod()
                .WithExposedHeaders("X-Total-Count")));
    }

    public static void AddJwtBearerAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
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

    public static void AddGraphQL(this IServiceCollection services, WebApplicationBuilder builder)
    {
        services
            .AddGraphQLServer()
            .AddAuthorization()
            .AddQueryType<RootQuery>()
            .AddMutationType<RootMutation>()
            .AddErrorFilter(error => error.WithMessage(error.Exception?.Message ?? error.Message))
            .ModifyRequestOptions(o => o.IncludeExceptionDetails = builder.Environment.IsDevelopment());
    }
}
