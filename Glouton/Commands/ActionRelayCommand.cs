using System;

namespace Glouton.Commands;

public class ActionRelayCommand : BaseRelayCommand
{
    private readonly Action _action;

    public ActionRelayCommand(Action action)
    {
        _action = action;
    }

    public override void Execute(object? parameter)
    {
        _action.Invoke();
    }
}