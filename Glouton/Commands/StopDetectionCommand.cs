using Glouton.Features.FileManagement.FileDetection;
using Glouton.Interfaces;

namespace Glouton.Commands;

public class StopDetectionCommand : BaseRelayCommand
{
    private readonly IFileDetection _fileDetection;

    public StopDetectionCommand(IFileDetection fileDetection)
    {
        _fileDetection = fileDetection;
    }

    public override bool CanExecute(object? parameter)
    {
        return _fileDetection.State ==  EFileDetectionState.Started || _fileDetection.State == EFileDetectionState.Enabled;
    }

    public override void Execute(object? parameter)
    {
        _fileDetection.StopDetection();
    }
}