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

    private FileSystemWatcher? _fileWatcher;
    private CompositeDisposable? _subsriptions;

    public event FileSystemEventHandler? Created;

    public bool IsEnabled
    {
        get => IsStarted && _fileWatcher != null && _fileWatcher.EnableRaisingEvents;
    }

    public bool IsStarted {  get; private set; }

    public FileWatcherService(IFileEventDispatcher  dispatcher)
    {
        _dispatcher = dispatcher;  
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

        _fileWatcher.Error +=_fileWatcher_Error;
        _fileWatcher.EnableRaisingEvents = true;
    }

    private void _fileWatcher_Error(object sender, ErrorEventArgs e)
    {
        var ee = e;
    }

    private void OnFileChanged(object sender, FileSystemEventArgs e)
    {
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
            throw new ArgumentException("folder does not exist", nameof(folder));
        }
    }

    public void Dispose()
    {
        _fileWatcher?.Dispose();
        _subsriptions?.Dispose();
    }
}