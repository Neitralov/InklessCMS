namespace Database.Repositories;

public abstract class BaseRepository(DatabaseContext database)
{
    public virtual async Task SaveChangesAsync() => await database.SaveChangesAsync();
}
