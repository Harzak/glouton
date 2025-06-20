using System;
using System.Windows.Input;

namespace Glouton.Commands;

public abstract class BaseRelayCommand : ICommand
{

    public event EventHandler? CanExecuteChanged
    {
        add
        {
            CommandManager.RequerySuggested += value;
        }

        remove
        {
            CommandManager.RequerySuggested -= value;
        }
    }

    public virtual bool CanExecute(object? parameter)
    {
        return true;
    }

    public abstract void Execute(object? parameter);

    private void CommandBase_RaiseCanExecuteChanged(object? sender, System.EventArgs e)
    {
        CommandManager.InvalidateRequerySuggested();
    }
}