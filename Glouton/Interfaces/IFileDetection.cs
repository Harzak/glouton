using Glouton.EventArgs;
using Glouton.Features.FileManagement.FileDetection;
using System;

namespace Glouton.Interfaces;

public interface IFileDetection : IDisposable
{
    event EventHandler<FileDetectionStateEventArgs>? StatusChanged;

    EFileDetectionState State { get; }

    void StartDetection(string location);
    void StopDetection();
    void InvokeOnFileDetected(Action<DetectedFileEventArgs> action);
}