namespace Database;

public sealed class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        BuildArticles(modelBuilder.Entity<Article>());
        BuildCollections(modelBuilder.Entity<Collection>());
        BuildUsers(modelBuilder.Entity<User>());
        BuildUserSessions(modelBuilder.Entity<UserSession>());


        static void BuildArticles(EntityTypeBuilder<Article> entity)
        {
            entity.ToTable(nameof(Article) + "s");

            entity
                .Property(p => p.ArticleId)
                .HasMaxLength(Article.MaxIdLength);
            entity.HasKey(nameof(Article.ArticleId));

            entity
                .Property(p => p.Title)
                .IsRequired()
                .HasMaxLength(Article.MaxTitleLength);

            entity
                .Property(p => p.Description)
                .IsRequired()
                .HasMaxLength(Article.MaxDescriptionLength);

            entity
                .Property(p => p.Text)
                .IsRequired();

            entity
                .Property(p => p.IsPublished)
                .IsRequired();

            entity.Property(p => p.PublishDate);

            entity
                .Property(p => p.CreationDate)
                .IsRequired();

            entity
                .Property(p => p.Views)
                .IsRequired();

            entity
                .Property(p => p.IsPinned)
                .IsRequired();
        }

        static void BuildCollections(EntityTypeBuilder<Collection> entity)
        {
            entity.ToTable(nameof(Collection) + "s");

            entity
                .Property(p => p.CollectionId)
                .HasMaxLength(Collection.MaxIdLength);
            entity.HasKey(nameof(Collection.CollectionId));

            entity
                .Property(p => p.Title)
                .HasMaxLength(Collection.MaxTitleLength)
                .IsRequired();

            entity
                .HasMany(p => p.Articles)
                .WithMany();
        }

        static void BuildUsers(EntityTypeBuilder<User> entity)
        {
            entity.ToTable(nameof(User) + "s");

            entity.Property(p => p.UserId);
            entity.HasKey(nameof(User.UserId));

            entity
                .Property(p => p.Email)
                .IsRequired();

            entity
                .Property(p => p.PasswordHash)
                .IsRequired();

            entity
                .Property(p => p.PasswordSalt)
                .IsRequired();

            entity
                .Property(p => p.CanManageArticles)
                .IsRequired();
        }

        static void BuildUserSessions(EntityTypeBuilder<UserSession> entity)
        {
            entity.ToTable(nameof(UserSession) + "s");

            entity.Property(p => p.UserSessionId);
            entity.HasKey(nameof(UserSession.UserSessionId));

            entity
                .Property(p => p.UserId)
                .IsRequired();

            entity
                .ComplexProperty(p => p.RefreshToken)
                .IsRequired();

            entity
                .Property(p => p.ExpirationDate)
                .IsRequired();
        }
    }
}
