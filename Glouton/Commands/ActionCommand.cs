using System;

namespace Glouton.Commands;

public class ActionCommand : BaseCommand
{
    private Action _action;

    public ActionCommand(Action action)
    {
        _action = action;
    }

    public override void Execute(object? parameter)
    {
        _action.Invoke();
    }
}