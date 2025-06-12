using Glouton.Features.FileManagement.FileWatcher;
using System;
using System.IO;

namespace Glouton.Interfaces;

public interface IFileWatcherService : IDisposable
{
    event FileSystemEventHandler? FileChanged;
    event EventHandler<FileWatcherStateEventArgs>? StatusChanged;

    EFileWatcherState State { get; }

    void StartWatcher(string location);
    void StopWatcher();
}