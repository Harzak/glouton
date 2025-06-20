using Glouton.EventArgs;
using Glouton.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glouton.Features.FileManagement.FileDetection;

public sealed class FileDetectionCoordinator : IFileDetection
{
    private readonly ILoggingService _logger;
    private readonly IFileEventDispatcher _dispatcher;

    private FileScan? _scanner;
    private FileWatcher? _watcher;

    private readonly ConcurrentDictionary<string, FileTrackingInfo> _trackedFiles;
    private readonly object _lock;


    public event EventHandler<FileDetectionStateEventArgs>? StatusChanged;
    public event EventHandler<DetectedFileEventArgs>? FileDetected;

    public EFileDetectionState State { get; private set; }

    public FileDetectionCoordinator(IFileEventDispatcher dispatcher, ILoggingService logger)
    {
        _dispatcher = dispatcher;
        _logger = logger;
        _trackedFiles = [];
        _lock = new object();
        this.State = EFileDetectionState.Enabled;
    }

    public void StartDetection(string location)
    {
        if (this.State != EFileDetectionState.Started)
        {
            _scanner = new FileScan(location, ScanPolicy.SlowScanPolicy);
            _scanner.FileDetected += this.OnScanFileDetected;
            _scanner.Start();

            _watcher = new FileWatcher(location);
            _watcher.FileDetected += this.OnWatcherFileDetected;
            _watcher.Error += this.OnError;
            _watcher.Start();

            _logger.LogInfo($"File detection enabled for: '{location}'.");
            this.SetState(EFileDetectionState.Started);
        }
    }

    public void StopDetection()
    {
        if (this.State == EFileDetectionState.Started)
        {
            _scanner?.Dispose();
            _scanner = null;
            _watcher?.Dispose();
            _watcher = null;
            this.SetState(EFileDetectionState.Stopped);
        }
    }

    private void OnScanFileDetected(object? sender, DetectedFileEventArgs e)
    {
        ProcessDetectedFile(e);
    }

    private void OnWatcherFileDetected(object? sender, DetectedFileEventArgs e)
    {
        ProcessDetectedFile(e);
    }

    private void ProcessDetectedFile(DetectedFileEventArgs e)
    {
        FileInfoSnapshot? fileInfo = GetCurrentFileInfo(e.FilePath);
        string fileKey = e.FilePath.ToUpperInvariant();

        lock (_lock)
        {
            bool shouldDispatch = false;
            DateTime now = DateTime.UtcNow;

            if (_trackedFiles.TryGetValue(fileKey, out FileTrackingInfo? existing))
            {
                if (fileInfo != null && IsFileRecreated(existing, fileInfo))
                {
                    shouldDispatch = true;
                    existing.UpdateFileInfo(fileInfo, now);
                }
                else if (!existing.WasDispatchedRecently(now))
                {
                    shouldDispatch = true;
                    existing.LastSeenUtc = now;
                }
                else
                {
                    existing.LastSeenUtc =now;
                }
            }
            else
            {
                shouldDispatch = fileInfo != null;
                if (fileInfo != null)
                {
                    _trackedFiles[fileKey] = new FileTrackingInfo(fileInfo)
                    {
                        LastSeenUtc = now
                    };
                }
            }

            if (shouldDispatch && fileInfo != null)
            {
                _trackedFiles[fileKey].LastDispatchedUtc = now;
                this.DispatchFileEvent(e);
            }
        }
    }

    private static bool IsFileRecreated(FileTrackingInfo existing, FileInfoSnapshot current)
    {
        return current.CreationTimeUtc > existing.FileInfo.CreationTimeUtc ||
               (current.CreationTimeUtc == existing.FileInfo.CreationTimeUtc &&
               current.LastWriteTimeUtc != existing.FileInfo.LastWriteTimeUtc &&
               current.Length != existing.FileInfo.Length);
    }

    private FileInfoSnapshot? GetCurrentFileInfo(string filePath)
    {
        FileInfo fileInfo = new(filePath);
        return fileInfo.Exists
            ? new FileInfoSnapshot(fileInfo.CreationTimeUtc, fileInfo.LastWriteTimeUtc, fileInfo.Length)
            : null;
    }

    private void DispatchFileEvent(DetectedFileEventArgs e)
    {
        _logger.LogInfo($"File has been detected", e.FilePath);
        _dispatcher.BeginInvoke(e, new Action(() =>
        {
            if (Directory.Exists(e.FilePath) || File.Exists(e.FilePath))
            {
                FileDetected?.Invoke(this, e);
            }
        }));
    }


    private void CleanupExpiredEntries(object? state)
    {
        DateTime cutoff = DateTime.UtcNow.AddMinutes(-30);
        List<string> expiredKeys = [];

        lock (_lock)
        {
            foreach (KeyValuePair<string, FileTrackingInfo> file in _trackedFiles)
            {
                if (file.Value.LastSeenUtc < cutoff)
                {
                    expiredKeys.Add(file.Key);
                }
            }

            foreach (string key in expiredKeys)
            {
                _trackedFiles.TryRemove(key, out _);
            }
        }
    }

    private void OnError(object sender, ErrorEventArgs e)
    {
        _logger.LogError(e.GetException().Message);
    }

    private void SetState(EFileDetectionState state)
    {
        if (this.State != state)
        {
            State = state;
            StatusChanged?.Invoke(this, new FileDetectionStateEventArgs(state));
            _logger.LogInfo($"File watcher is now {state}.");
        }
    }

    public void Dispose()
    {
        _scanner?.Dispose();
        _watcher?.Dispose();
        _dispatcher.Dispose();
    }
}

public enum EFileDetectionState
{
    Unknown = 0,
    Enabled = 1,
    Started = 2,
    Stopped = 3
}