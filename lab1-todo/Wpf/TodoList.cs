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
}


