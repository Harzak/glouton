using Glouton.Features.FileManagement.FileDetection;
using Glouton.Interfaces;

namespace Glouton.Commands;

public class StarDetectionCommand : BaseRelayCommand
{
    private readonly IFileDetection _fileDetection;
    private readonly ISettingsService _settingsService;

    public StarDetectionCommand(IFileDetection fileDetection, ISettingsService settingsService)
    {
        _fileDetection = fileDetection;
        _settingsService = settingsService;
    }

    public override bool CanExecute(object? parameter)
    {
        return _fileDetection.State == EFileDetectionState.Stopped;
    }

    public override void Execute(object? parameter)
    {
        _fileDetection.StartDetection(_settingsService.GetSettings().WatchedFilePath);
    }
}