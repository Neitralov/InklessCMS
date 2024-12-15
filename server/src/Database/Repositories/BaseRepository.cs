namespace Database.Repositories;

public abstract class BaseRepository(DatabaseContext database)
{
    public virtual async Task SaveChanges() => await database.SaveChangesAsync();
}