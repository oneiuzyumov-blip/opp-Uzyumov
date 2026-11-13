namespace PaintLab;

public interface ICommand
{
    void Execute();
    void Undo();
}

public class AddShapeCommand : ICommand
{
    private readonly Drawing _drawing;
    private readonly Shape _shape;

    public AddShapeCommand(Drawing drawing, Shape shape)
    {
        _drawing = drawing;
        _shape = shape;
    }

    public void Execute() => _drawing.Add(_shape);
    public void Undo() => _drawing.Remove(_shape);
}

public class RemoveShapeCommand : ICommand
{
    private readonly Drawing _drawing;
    private readonly Shape _shape;
    private int _index;

    public RemoveShapeCommand(Drawing drawing, Shape shape)
    {
        _drawing = drawing;
        _shape = shape;
    }

    public void Execute()
    {
        _index = _drawing.IndexOf(_shape);
        _drawing.Remove(_shape);
    }

    public void Undo() => _drawing.Insert(_index, _shape);
}

public class ClearCommand : ICommand
{
    private readonly Drawing _drawing;
    private List<Shape> _backup = new();

    public ClearCommand(Drawing drawing) => _drawing = drawing;

    public void Execute()
    {
        _backup = _drawing.Shapes.ToList();
        _drawing.Clear();
    }

    public void Undo()
    {
        foreach (var shape in _backup)
            _drawing.Add(shape);
    }
}

public class History
{
    private readonly Stack<ICommand> _undo = new();
    private readonly Stack<ICommand> _redo = new();

    public void Do(ICommand command)
    {
        command.Execute();
        _undo.Push(command);
        _redo.Clear();
    }

    public void Undo()
    {
        if (_undo.Count == 0) return;
        var command = _undo.Pop();
        command.Undo();
        _redo.Push(command);
    }

    public void Redo()
    {
        if (_redo.Count == 0) return;
        var command = _redo.Pop();
        command.Execute();
        _undo.Push(command);
    }

    public void Clear()
    {
        _undo.Clear();
        _redo.Clear();
    }
}
