using Microsoft.Extensions.Logging;
using TaskListApp.Services;
using TaskListApp.ViewModels;

namespace TaskListApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>();

        string databasePath = Path.Combine(
            FileSystem.AppDataDirectory,
            "tasks.db3");

        builder.Services.AddSingleton<ITodoService>(
            new TodoService(databasePath));

        builder.Services.AddSingleton<MainViewModel>();
        builder.Services.AddSingleton<MainPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}