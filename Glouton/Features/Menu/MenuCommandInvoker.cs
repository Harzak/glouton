using Glouton.Interfaces;
using System;
using System.Collections.Generic;

namespace Glouton.Features.Menu;

internal class MenuCommandInvoker : IMenuCommandInvoker
{
    private readonly Stack<IMenuCommand> _commandHistory = new Stack<IMenuCommand>();

    public MenuCommandInvoker()
    {
    }

    public void ExecuteCommand(IMenuCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (command.CanExecute())
        {
            command.Execute();
            _commandHistory.Push(command);
        }
    }

    public void UndoLastCommand()
    {
        if (_commandHistory.Count > 0)
        {
            var lastCommand = _commandHistory.Pop();
            lastCommand.Undo();
        }
    }

    public bool CanUndo => _commandHistory.Count > 0;
}