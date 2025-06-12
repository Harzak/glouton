using Glouton.Features.FileManagement.FileWatcher;
using Glouton.Interfaces;

namespace Glouton.Commands;

public class StopWatcherCommand : BaseRelayCommand
{
    private readonly IFileWatcherService _fileWatcherService;

    public StopWatcherCommand(IFileWatcherService fileWatcherService)
    {
        _fileWatcherService = fileWatcherService;
    }

    public override bool CanExecute(object? parameter)
    {
        return _fileWatcherService.State ==  EFileWatcherState.Started || _fileWatcherService.State == EFileWatcherState.Enabled;
    }

    public override void Execute(object? parameter)
    {
        _fileWatcherService.StopWatcher();
    }
}