using Glouton.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Windows;

namespace Glouton.Features.Loging;

internal sealed class AppLogger : ILoggingService
{
    private readonly ConcurrentQueue<LogEntry> _entries;

    public event EventHandler<LogEntryEventArgs>? LogEntryAdded;

    public AppLogger()
    {
        _entries = [];
    }

    public void LogInfo(string message, string fileName = "")
    {
        AddLog(LogLevel.Information, message, fileName);
    }

    public void LogWarning(string message, string fileName = "")
    {
        AddLog(LogLevel.Warning, message, fileName);
    }

    public void LogError(string message, string fileName = "")
    {
        AddLog(LogLevel.Error, message, fileName);
    }

    public void LogDebug(string message, string fileName = "")
    {
        AddLog(LogLevel.Debug, message, fileName);
    }

    private void AddLog(LogLevel level, string message, string fileName = "")
    {
        LogEntry entry = new(level, message, fileName);
        _entries.Enqueue(entry);

        Application.Current?.Dispatcher.BeginInvoke(() =>
        {
            LogEntryAdded?.Invoke(this, new LogEntryEventArgs(entry));
        });
    }

    public IEnumerable<LogEntry> GetAllLogs()
    {
        return _entries.ToArray();
    }

    public void ClearLogs()
    {
        while (_entries.TryDequeue(out _)) { }
    }
}
