using Glouton.Features.FileManagement.FileWatcher;
using Glouton.Interfaces;

namespace Glouton.Commands;

public class StartWatcherCommand : BaseRelayCommand
{
    private readonly IFileWatcherService _fileWatcherService;
    private readonly ISettingsService _settingsService;

    public StartWatcherCommand(IFileWatcherService fileWatcherService, ISettingsService settingsService)
    {
        _fileWatcherService = fileWatcherService;
        _settingsService = settingsService;
    }

    public override bool CanExecute(object? parameter)
    {
        return _fileWatcherService.State == EFileWatcherState.Stopped;
    }

    public override void Execute(object? parameter)
    {
        _fileWatcherService.StartWatcher(_settingsService.GetSettings().WatchedFilePath);
    }
}