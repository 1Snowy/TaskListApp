using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using TaskListApp.Models;
using TaskListApp.Services;

namespace TaskListApp.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly ITodoService service;

    public ObservableCollection<TodoItem> Tasks { get; } = new();
    public ObservableCollection<TodoItem> ParentTasks { get; } = new();

    public ObservableCollection<string> Categories { get; } = new()
    {
        "Учёба",
        "Работа",
        "Покупки",
        "Личное"
    };

    [ObservableProperty]
    private string newTaskTitle = "";

    [ObservableProperty]
    private string selectedCategory = "Учёба";

    [ObservableProperty]
    private TodoItem? selectedParent;

    public MainViewModel(ITodoService service)
    {
        this.service = service;
    }

    public async Task LoadAsync()
    {
        await service.InitializeAsync();

        var items = await service.GetAllAsync();

        Tasks.Clear();
        ParentTasks.Clear();

        ParentTasks.Add(new TodoItem
        {
            Id = 0,
            Title = "Основная задача"
        });

        // 1. Сначала берем только основные задачи (где ParentId == 0)
        var mainTasks = items.Where(x => x.ParentId == 0).ToList();

        foreach (var mainTask in mainTasks)
        {
            // Добавляем основную задачу в общий список и в список для выбора родителя
            Tasks.Add(mainTask);
            ParentTasks.Add(mainTask);

            // 2. Ищем все подзадачи для этой конкретной задачи и добавляем сразу за ней
            var subTasks = items.Where(x => x.ParentId == mainTask.Id);
            foreach (var subTask in subTasks)
            {
                Tasks.Add(subTask);
            }
        }

        SelectedParent = ParentTasks[0];
    }

    [RelayCommand]
    private async Task AddTaskAsync()
    {
        if (string.IsNullOrWhiteSpace(NewTaskTitle))
            return;

        var item = new TodoItem
        {
            Title = NewTaskTitle.Trim(),
            Category = SelectedCategory,
            ParentId = SelectedParent?.Id ?? 0
        };

        await service.AddAsync(item);

        NewTaskTitle = "";
        await LoadAsync();
    }

    [RelayCommand]
    private async Task ToggleTaskAsync(TodoItem item)
    {
        item.IsCompleted = !item.IsCompleted;

        await service.UpdateAsync(item);
        await LoadAsync();
    }

    [RelayCommand]
    private async Task DeleteTaskAsync(TodoItem item)
    {
        await service.DeleteAsync(item);
        await LoadAsync();
    }
}