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

    public event FileSystemEventHandler? FileChanged;
    public event EventHandler<FileWatcherStateEventArgs>? StatusChanged;

    public EFileWatcherState State { get; private set; }

    public FileWatcherService(IFileEventDispatcher dispatcher, ILoggingService logger)
    {
        _dispatcher = dispatcher;
        _logger = logger;
    }

    public void StartWatcher(string location)
    {
        if (this.State != EFileWatcherState.Started)
        {
            this.Subscribe(location);
            this.SetState(EFileWatcherState.Started);
        }
    }

    public void StopWatcher()
    {
        if (this.State == EFileWatcherState.Started)
        {
            _fileWatcher?.Dispose();
            _subsriptions?.Dispose();
            _fileWatcher = null;
            _subsriptions = null;
            this.SetState(EFileWatcherState.Stopped);
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

        _logger.LogInfo($"File watcher monitors: '{location}'.");
    }

    private void OnError(object sender, ErrorEventArgs e)
    {
        _logger.LogError(e.GetException().Message);
    }

    private void OnFileChanged(object sender, FileSystemEventArgs e)
    {
        _logger.LogInfo($"File {e.ChangeType}.", e.Name ?? "");
        _dispatcher.BeginInvoke(e, new Action(() =>
        {
            if ((Directory.Exists(e.FullPath) || File.Exists(e.FullPath)))
            {
                FileChanged?.Invoke(this, e);
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
                InternalBufferSize = 64 * 1024
            };
        }
        else
        {
            throw new ArgumentException("Folder does not exist", nameof(folder));
        }
    }

    private void SetState(EFileWatcherState state)
    {
        if (this.State != state)
        {
            State = state;
            StatusChanged?.Invoke(this, new FileWatcherStateEventArgs(state));
            _logger.LogInfo($"File watcher is now {state}.");
        }
    }

    public void Dispose()
    {
        _fileWatcher?.Dispose();
        _subsriptions?.Dispose();
    }
}

public enum EFileWatcherState
{
    Unknown = 0,
    Enabled = 1,
    Started = 2,
    Stopped = 3
}