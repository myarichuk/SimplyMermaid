namespace MermaidEditor.Services;

using System.Collections.Generic;

public interface ICommand
{
    void Execute();
    void Undo();
}

public class CommandManager
{
    public Stack<ICommand> UndoStack { get; } = new();
    public Stack<ICommand> RedoStack { get; } = new();

    public void ExecuteCommand(ICommand command)
    {
        command.Execute();
        UndoStack.Push(command);
        RedoStack.Clear();
    }

    public void Undo()
    {
        if (UndoStack.Count > 0)
        {
            var command = UndoStack.Pop();
            command.Undo();
            RedoStack.Push(command);
        }
    }

    public void Redo()
    {
        if (RedoStack.Count > 0)
        {
            var command = RedoStack.Pop();
            command.Execute();
            UndoStack.Push(command);
        }
    }

    public void Clear()
    {
        UndoStack.Clear();
        RedoStack.Clear();
    }
}
