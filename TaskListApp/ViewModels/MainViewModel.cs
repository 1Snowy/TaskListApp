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

        // Заглушка для пикера «родителя». Id = 0 — служебное значение только для UI:
        // при сохранении оно превращается в null (задача без родителя).
        ParentTasks.Add(new TodoItem
        {
            Id = 0,
            Title = "Основная задача"
        });

        // 1. Сначала берем только основные задачи (где ParentId == null)
        var mainTasks = items.Where(x => x.ParentId == null).ToList();

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
            // Заглушка пикера имеет Id = 0 — значит «без родителя», в БД уходит null.
            // Остальные значения — Id родителя: его целостность проверит внешний ключ.
            ParentId = SelectedParent == null || SelectedParent.Id == 0 ? null : SelectedParent.Id
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
