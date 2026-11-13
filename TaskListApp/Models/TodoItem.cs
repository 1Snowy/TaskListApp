using SQLite;

namespace TaskListApp.Models;

[Table("TodoItems")]
public class TodoItem
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Title { get; set; } = "";

    public string Category { get; set; } = "";

    public bool IsCompleted { get; set; }

    // 0 означает, что задача является основной.
    public int ParentId { get; set; }

    [Ignore]
    public string DisplayTitle
    {
        get
        {
            string prefix = ParentId == 0 ? "" : "    - ";
            string mark = IsCompleted ? "✓ " : "";
            return prefix + mark + Title;
        }
    }
}