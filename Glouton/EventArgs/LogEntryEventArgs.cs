using Glouton.Features.Loging;
using System;

namespace Glouton.EventArgs;

public class LogEntryEventArgs : System.EventArgs
{
    public LogEntry LogEntry { get; }

    public LogEntryEventArgs(LogEntry logEntry)
    {
        LogEntry = logEntry;
    }
}

