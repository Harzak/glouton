using Glouton.Features.FileManagement.FileEvent;
using Glouton.Interfaces;
using System;
using System.CodeDom;
using System.IO;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Glouton.Features.FileManagement.FileWatcher;

internal sealed class FileWatcherService : IFileWatcherService
{
    private readonly IFileEventDispatcher _dispatcher;
    private readonly ILoggingService _logger;

    private FileSystemWatcher? _fileWatcher;
    private CompositeDisposable? _subsriptions;

    public event FileSystemEventHandler? Created;

    public bool IsEnabled
    {
        get => IsStarted && _fileWatcher != null && _fileWatcher.EnableRaisingEvents;
    }

    public bool IsStarted { get; private set; }

    public FileWatcherService(IFileEventDispatcher dispatcher, ILoggingService logger)
    {
        _dispatcher = dispatcher;
        _logger = logger;
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

        _subsriptions = new CompositeDisposable
        {
            Observable.FromEventPattern<FileSystemEventArgs>(_fileWatcher, "Changed")
                    .Select(pattern => pattern.EventArgs)
                    .DistinctUntilChanged(args => args, new FileSystemEventArgsEqualityComparer())
                    .Subscribe((e) => OnFileChanged(this, e))
        };

        _fileWatcher.Error +=OnError;
        _fileWatcher.EnableRaisingEvents = true;

        _logger.LogInfo($"File watcher started for {location}.");
    }

    private void OnError(object sender, ErrorEventArgs e)
    {
        _logger.LogError(e.GetException().Message);
    }

    private void OnFileChanged(object sender, FileSystemEventArgs e)
    {
        _logger.LogInfo($"File changed ({e.ChangeType}).", e.Name ?? "");
        _dispatcher.BeginInvoke(e, new Action(() =>
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
            throw new ArgumentException("Folder does not exist", nameof(folder));
        }
    }

    public void Dispose()
    {
        _fileWatcher?.Dispose();
        _subsriptions?.Dispose();
    }
}