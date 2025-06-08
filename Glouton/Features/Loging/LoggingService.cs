using Glouton.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Windows;

namespace Glouton.Features.Loging;

internal sealed class LoggingService : ILoggingService
{
    private readonly ConcurrentQueue<LogEntry> _entries;

    public event EventHandler<LogEntryEventArgs>? LogEntryAdded;

    public LoggingService()
    {
        _entries = [];
    }

    public void LogInfo(string message)
    {
        AddLog(LogLevel.Information, message);
    }

    public void LogWarning(string message)
    {
        AddLog(LogLevel.Warning, message);
    }

    public void LogError(string message)
    {
        AddLog(LogLevel.Error, message);
    }

    public void LogDebug(string message)
    {
        AddLog(LogLevel.Debug, message);
    }

    private void AddLog(LogLevel level, string message)
    {
        LogEntry entry = new(level, message);
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
