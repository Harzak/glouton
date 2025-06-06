using Glouton.Features.FileManagement.FileEvent;
using System;
using System.IO;

namespace Glouton.Features.FileManagement.FileWatcher;

internal sealed class FileWatcherService
{
    private FileSystemWatcher? _fileWatcher;

    public event FileSystemEventHandler? Created;

    public bool IsEnabled
    {
        get => IsStarted && _fileWatcher != null && _fileWatcher.EnableRaisingEvents;
    }

    public bool IsStarted { private get; set; }

    public FileWatcherService()
    {

    }

    public void Start(string location)
    {
        if (!IsStarted)
        {
            this.Subscribe(location);
            this.IsStarted = true;
        }
    }

    private void Subscribe(string location)
    {
        _fileWatcher = CreateFileWatcher(location);
        _fileWatcher.Created += OnFileCreated;
        _fileWatcher.Changed += OnFileCreated;
        _fileWatcher.EnableRaisingEvents = true;
    }

    private void OnFileCreated(object sender, FileSystemEventArgs e)
    {
        FileEventDispatcher.Current.BeginInvoke(e, new Action(() =>
        {
            if ((Directory.Exists(e.FullPath) || File.Exists(e.FullPath)))
            {
                Created?.Invoke(this, e);
            }
        }));
    }

    private static FileSystemWatcher CreateFileWatcher(string folder)
    {
        if (Directory.Exists(folder))
        {
            return new FileSystemWatcher(folder)
            {
                Filter = "*.*",
                NotifyFilter = NotifyFilters.FileName
                    | NotifyFilters.LastWrite
                    | NotifyFilters.DirectoryName,
                IncludeSubdirectories = true,
                InternalBufferSize = 64 * 1024 // max buffer size in bytes
            };
        }
        else
        {
            throw new ArgumentException("folder does not exist", nameof(folder));
        }
    }

    public void Dispose()
    {
        _fileWatcher?.Dispose();
    }
}