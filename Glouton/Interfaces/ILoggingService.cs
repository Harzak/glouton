using Glouton.Features.Loging;
using System;
using System.Collections.Generic;

namespace Glouton.Interfaces;

public interface ILoggingService
{
    event EventHandler<LogEntryEventArgs>? LogEntryAdded;
    void LogInfo(string message);
    void LogWarning(string message);
    void LogError(string message);
    void LogDebug(string message);
    IEnumerable<LogEntry> GetAllLogs();
    void ClearLogs();
}