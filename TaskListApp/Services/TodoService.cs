using SQLite;
using TaskListApp.Models;

namespace TaskListApp.Services;

public class TodoService : ITodoService
{
    private readonly SQLiteAsyncConnection database;

    public TodoService(string databasePath)
    {
        database = new SQLiteAsyncConnection(databasePath);
    }

    public async Task InitializeAsync()
    {
        await EnableForeignKeysAsync();
        await database.ExecuteAsync(@"
            CREATE TABLE IF NOT EXISTS ""TodoItems"" (
                ""Id"" integer primary key autoincrement,
                ""Title"" varchar,
                ""Category"" varchar,
                ""IsCompleted"" integer not null,
                ""ParentId"" integer,
                FOREIGN KEY (""ParentId"") REFERENCES ""TodoItems"" (""Id"") ON DELETE CASCADE
            );");

        // Индекс по внешнему ключу: ускоряет выборку подзадач (WHERE ParentId = ?).
        await database.ExecuteAsync(
            @"CREATE INDEX IF NOT EXISTS ""IX_TodoItems_ParentId"" ON ""TodoItems"" (""ParentId"");");
    }

    private Task EnableForeignKeysAsync()
    {
        return database.ExecuteAsync("PRAGMA foreign_keys = ON;");
    }

    public async Task<List<TodoItem>> GetAllAsync()
    {
        return await database.Table<TodoItem>()
            .OrderBy(item => item.Id)
            .ToListAsync();
    }

    public async Task AddAsync(TodoItem item)
    {
        await EnableForeignKeysAsync();
        await database.InsertAsync(item);
    }

    public async Task UpdateAsync(TodoItem item)
    {
        await EnableForeignKeysAsync();
        await database.UpdateAsync(item);
    }

    public async Task DeleteAsync(TodoItem item)
    {
        await EnableForeignKeysAsync();

        await database.DeleteAsync(item);
    }

    public async Task CloseAsync()
    {
        await database.CloseAsync();
    }
}
