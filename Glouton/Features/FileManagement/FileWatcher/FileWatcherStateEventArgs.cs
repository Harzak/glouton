using System;

namespace Glouton.Features.FileManagement.FileWatcher;

public class FileWatcherStateEventArgs : EventArgs
{
    public EFileWatcherState State { get; }

    public FileWatcherStateEventArgs(EFileWatcherState state)
    {
        this.State = state;
    }
}