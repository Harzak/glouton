using Glouton.Features.Menu;
using Glouton.Interfaces;

namespace Glouton.Commands;

public class MenuRelayCommand : BaseRelayCommand
{
    private readonly IMenuCommand _command;
    private readonly IMenuCommandInvoker _invoker;

    public MenuRelayCommand(IMenuCommand command, IMenuCommandInvoker invoker)
    {
        _command = command;
        _invoker = invoker;
    }

    public override void Execute(object? parameter)
    {
        _invoker.ExecuteCommand(_command);
    }

    public override bool CanExecute(object? parameter)
    {
        return _command.CanExecute();
    }
}