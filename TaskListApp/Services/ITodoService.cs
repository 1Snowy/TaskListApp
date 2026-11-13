using TaskListApp.Models;

namespace TaskListApp.Services;

public interface ITodoService
{
    Task InitializeAsync();
    Task<List<TodoItem>> GetAllAsync();
    Task AddAsync(TodoItem item);
    Task UpdateAsync(TodoItem item);
    Task DeleteAsync(TodoItem item);
    Task CloseAsync();
}