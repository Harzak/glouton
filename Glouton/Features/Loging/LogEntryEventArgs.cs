using System;

namespace Glouton.Features.Loging;

public class LogEntryEventArgs : EventArgs
{
    public LogEntry LogEntry { get; }

    public LogEntryEventArgs(LogEntry logEntry)
    {
        LogEntry = logEntry;
    }
}

