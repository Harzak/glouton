using Glouton.EventArgs;
using Glouton.Features.Loging;
using System;
using System.Collections.Generic;

namespace Glouton.Interfaces;

public interface ILoggingService
{
    event EventHandler<LogEntryEventArgs>? LogEntryAdded;
    void LogInfo(string message, string fileName = "");
    void LogWarning(string message, string fileName = "");
    void LogError(string message, string fileName = "");
    void LogDebug(string message, string fileName = "");
    IEnumerable<LogEntry> GetAllLogs();
    void ClearLogs();
}