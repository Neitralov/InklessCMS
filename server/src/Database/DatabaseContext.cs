namespace Database;

public sealed class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
{
    public DbSet<Article> Articles => Set<Article>();
    public DbSet<Collection> Collections => Set<Collection>();
    public DbSet<User> Users => Set<User>();
    public DbSet<UserSession> UserSessions => Set<UserSession>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var admin = User.Create(
            email: "admin@example.ru",
            password: "admin",
            canManageArticles: true);

        modelBuilder.Entity<User>().HasData(admin.Value);
        
        modelBuilder.Entity<Collection>()
            .HasMany(p => p.Articles)
            .WithMany();

        modelBuilder.Entity<UserSession>()
            .ComplexProperty(p => p.RefreshToken);
    }
}