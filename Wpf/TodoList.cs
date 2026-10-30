using System.IO;
using System.Text.Json;

namespace Wpf;

public class TodoList
{
    private List<TodoItem> _items = new List<TodoItem>();
    public IReadOnlyList<TodoItem> Items => _items;
    public void Add(string title, int priority)
    {
        TodoItem item = new TodoItem(title, false, priority);
        _items.Add(item);

    }
    public void Remove(TodoItem item)
    {
        bool removed = _items.Remove(item);
        if (!removed)
            throw new ArgumentException("Такой задачи нет в списке");
    }

    public void Save(string path)
    {
        JsonSerializerOptions options = new JsonSerializerOptions();
        options.WriteIndented = true; 


        string json = JsonSerializer.Serialize(_items, options);
        File.WriteAllText(path, json);
    }

    public void Load(string path)
    {
        _items.Clear();

        if (!File.Exists(path))
            return;

        string json = File.ReadAllText(path);
        List<TodoItem>? loaded = JsonSerializer.Deserialize<List<TodoItem>>(json);

        if (loaded != null)
            _items = loaded;
    }
}


