using TaskListApp.Models;
using TaskListApp.Services;
using Xunit;

namespace TaskListApp.Tests;

public class TodoServiceTests
{
    private static string CreateDatabasePath()
    {
        return Path.Combine(
            Path.GetTempPath(),
            Guid.NewGuid().ToString() + ".db3");
    }

    [Fact]
    public async Task AddTask_ShouldSaveTask()
    {
        string path = CreateDatabasePath();
        var service = new TodoService(path);

        try
        {
            await service.InitializeAsync();

            await service.AddAsync(new TodoItem
            {
                Title = "Тестовая задача",
                Category = "Учёба"
            });

            var tasks = await service.GetAllAsync();

            Assert.Single(tasks);
            Assert.Equal("Тестовая задача", tasks[0].Title);
        }
        finally
        {
            await service.CloseAsync();
            if (File.Exists(path))
                File.Delete(path);
        }
    }

    [Fact]
    public async Task DeleteTask_ShouldRemoveTaskAndSubtasks()
    {
        string path = CreateDatabasePath();
        var service = new TodoService(path);

        try
        {
            await service.InitializeAsync();

            var parent = new TodoItem
            {
                Title = "Основная задача",
                Category = "Учёба"
            };

            await service.AddAsync(parent);

            await service.AddAsync(new TodoItem
            {
                Title = "Подзадача",
                Category = "Учёба",
                ParentId = parent.Id
            });

            await service.DeleteAsync(parent);

            var tasks = await service.GetAllAsync();

            Assert.Empty(tasks);
        }
        finally
        {
            await service.CloseAsync();
            if (File.Exists(path))
                File.Delete(path);
        }
    }
}
