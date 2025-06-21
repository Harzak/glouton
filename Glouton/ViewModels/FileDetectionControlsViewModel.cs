using Glouton.Commands;
using Glouton.EventArgs;
using Glouton.Features.FileManagement.FileDetection;
using Glouton.Interfaces;
using System.Windows.Input;

namespace Glouton.ViewModels;

public class FileDetectionControlsViewModel : BaseViewModel
{
    private readonly IFileDetection _fileDetection;

    public ICommand StartDetectionCommand { get; }
    public ICommand StopDetectionCommand { get; }

    public bool CanBeStopped => _fileDetection.State ==  EFileDetectionState.Enabled || _fileDetection.State == EFileDetectionState.Started;
    public bool CanBeStarted => _fileDetection.State ==  EFileDetectionState.Stopped;

    public FileDetectionControlsViewModel(IFileDetection fileDetection, ISettingsService settingsService)
    {
        _fileDetection = fileDetection;
        _fileDetection.StatusChanged += OnStatusChanged;

        StartDetectionCommand = new StarDetectionCommand(fileDetection, settingsService);
        StopDetectionCommand = new StopDetectionCommand(fileDetection);
    }

    private void OnStatusChanged(object? sender, FileDetectionStateEventArgs e)
    {
        base.OnPropertyChanged(nameof(CanBeStarted));
        base.OnPropertyChanged(nameof(CanBeStopped));
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _fileDetection.Dispose();
        }
        _fileDetection.StatusChanged -= OnStatusChanged;
        
        base.Dispose(disposing);
    }
}

