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
        await database.CreateTableAsync<TodoItem>();
    }

    public async Task<List<TodoItem>> GetAllAsync()
    {
        return await database.Table<TodoItem>()
            .OrderBy(item => item.Id)
            .ToListAsync();
    }

    public async Task AddAsync(TodoItem item)
    {
        await database.InsertAsync(item);
    }

    public async Task UpdateAsync(TodoItem item)
    {
        await database.UpdateAsync(item);
    }

    public async Task DeleteAsync(TodoItem item)
    {
        // При удалении основной задачи удаляем и её подзадачи.
        var subtasks = await database.Table<TodoItem>()
            .Where(task => task.ParentId == item.Id)
            .ToListAsync();

        foreach (var subtask in subtasks)
        {
            await database.DeleteAsync(subtask);
        }

        await database.DeleteAsync(item);
    }

    public async Task CloseAsync()
    {
        await database.CloseAsync();
    }
}
