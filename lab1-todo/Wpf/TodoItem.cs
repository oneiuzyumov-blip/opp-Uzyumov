using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Serialization;

namespace Wpf;

public class TodoItem
{
    private string _title = "";
    private bool _done;
    private int _priority;

    [JsonConstructor]
    public TodoItem(string title, bool isDone = false, int priority = 2)
    {
        Rename(title);
        SetPriority(priority);
        _done = isDone;
    }

    public string title => _title;
    public bool isDone => _done;
    public int priority => _priority;

    public void Complete()
    {
        _done = true;
    }

    public void Rename(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Не может быть пустым");
        _title = title.Trim();
    }
    public void SetPriority(int priority)
    {
        if (priority < 1 || priority > 3)
            throw new ArgumentException("Должно быть от 1 до 3");
        _priority = priority;

    }

    public override string ToString()
    {
        string mark = _done ? "[*]" : "[ ]";
        return mark + " " + _title + "(приоритет " + _priority + ")";
    }
}
