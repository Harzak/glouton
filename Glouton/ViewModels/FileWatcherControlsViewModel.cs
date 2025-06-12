using Glouton.Commands;
using Glouton.Features.FileManagement.FileWatcher;
using Glouton.Interfaces;
using System.Windows.Input;

namespace Glouton.ViewModels;

public class FileWatcherControlsViewModel : BaseViewModel
{
    private readonly IFileWatcherService _fileWatcherService;

    public ICommand StartWatcherCommand { get; }
    public ICommand StopWatcherCommand { get; }

    public bool CanBeStopped => _fileWatcherService.State == EFileWatcherState.Enabled || _fileWatcherService.State == EFileWatcherState.Started;
    public bool CanBeStarted => _fileWatcherService.State == EFileWatcherState.Stopped;

    public FileWatcherControlsViewModel(IFileWatcherService fileWatcherService, ISettingsService settingsService)
    {
        _fileWatcherService = fileWatcherService;
        _fileWatcherService.StatusChanged +=   OnStatusChanged;

        StartWatcherCommand = new StartWatcherCommand(fileWatcherService, settingsService);
        StopWatcherCommand = new StopWatcherCommand(fileWatcherService);
    }

    private void OnStatusChanged(object? sender, FileWatcherStateEventArgs e)
    {
        base.OnPropertyChanged(nameof(CanBeStarted));
        base.OnPropertyChanged(nameof(CanBeStopped));
    }
}

