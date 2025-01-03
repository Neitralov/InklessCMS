namespace WebAPI.IntegrationTests.ArticlesController.Endpoints;

[Collection("Tests")]
public sealed class GetPublishedArticlesTests(CustomWebApplicationFactory factory) 
    : ArticlesControllerTests, IAsyncLifetime
{
    public Task InitializeAsync() => Task.CompletedTask;
    
    public async Task DisposeAsync() => await factory.ResetDatabaseAsync();
    
    [Fact]
    public async Task EmptyListWillBeReturnedIfNoPublishedArticlesExist()
    {
        // TODO: Добавить статью черновик, чтобы проверить, что пользователь не получит лишние статьи
        // Arrange
        
        // Act
        
        // Assert
    }
    
    [Fact]
    public async Task PublishedArticlesWillBeReturnedIfPublishedArticlesExist()
    {
        // TODO: Добавить статью черновик, чтобы проверить, что пользователь не получит лишние статьи
        // Arrange
        
        // Act
        
        // Assert
    }
}