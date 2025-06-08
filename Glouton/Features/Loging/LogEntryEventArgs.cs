using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glouton.Features.Loging;

public class LogEntryEventArgs : EventArgs
{
    public LogEntry LogEntry { get; }

    public LogEntryEventArgs(LogEntry logEntry)
    {
        LogEntry = logEntry;
    }
}

