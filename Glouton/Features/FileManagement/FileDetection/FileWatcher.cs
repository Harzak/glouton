﻿using Glouton.EventArgs;
using Glouton.Utils.Result;
using System;
using System.IO;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Glouton.Features.FileManagement.FileDetection;

/// <summary>
/// Real-time file system watcher.
/// Provides event notifications when files are changed in the watched directory,
/// </summary>
internal sealed class FileWatcher : IDisposable
{
    private FileSystemWatcher? _systemFileWatcher;
    private CompositeDisposable? _subsriptions;

    public event EventHandler<DetectedFileEventArgs>? FileDetected;
    public event ErrorEventHandler? Error;

    private readonly string _location;

    public FileWatcher(string location)
    {
        _location = location;
    }

    public OperationResult Start()
    {
        if (string.IsNullOrEmpty(_location) || !Directory.Exists(_location))
        {
            return OperationResult.Error($"Invalid directory location specified: '{_location}'");
        }

        this.Subscribe();

        return OperationResult.Success;
    }

    private void Subscribe()
    {
        _systemFileWatcher = CreateFileWatcher(_location);

        _subsriptions = new CompositeDisposable
        {
            Observable.FromEventPattern<FileSystemEventArgs>(_systemFileWatcher, "Changed")
                    .Select(pattern => pattern.EventArgs)
                    .DistinctUntilChanged(args => args, new FileSystemEventArgsEqualityComparer())
                    .Subscribe((e) => OnFileChanged(this, e))
        };

        _systemFileWatcher.Error += Error;
        _systemFileWatcher.EnableRaisingEvents = true;
    }

    private void OnFileChanged(object sender, FileSystemEventArgs e)
    {
        FileDetected?.Invoke(this, new DetectedFileEventArgs(e.FullPath, DateTime.UtcNow));
    }

    private static FileSystemWatcher CreateFileWatcher(string folder)
    {
        if (Directory.Exists(folder))
        {
            return new FileSystemWatcher(folder)
            {
                Filter = "*.*",
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.DirectoryName,
                IncludeSubdirectories = true,
                InternalBufferSize = 64 * 1024
            };
        }
        else
        {
            throw new ArgumentException("Folder does not exist", nameof(folder));
        }
    }

    public void Dispose()
    {
        if (_systemFileWatcher != null)
        {
            _systemFileWatcher.Error -= Error; 
            _systemFileWatcher.Dispose();
        }
        _systemFileWatcher = null;
        _subsriptions?.Dispose();
        _subsriptions = null;
    }
}