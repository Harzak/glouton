using System;
using System.IO;

namespace Glouton.Interfaces;

public interface IFileWatcherService : IDisposable
{
    event FileSystemEventHandler? Created;
    bool IsEnabled { get; }
    bool IsStarted { get; }
    void Start(string location);
}