namespace Glouton.Interfaces;

public interface IMenuCommand
{
    void Execute();
    bool CanExecute();
    void Undo();
}