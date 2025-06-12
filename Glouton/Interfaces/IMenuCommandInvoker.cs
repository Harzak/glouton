namespace Glouton.Interfaces;

public interface IMenuCommandInvoker
{
    void ExecuteCommand(IMenuCommand command);
    void UndoLastCommand();
}