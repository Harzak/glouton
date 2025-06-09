using Microsoft.Extensions.Logging;
using System;

namespace Glouton.Features.Loging;

public record LogEntry
{
    public DateTime Timestamp { get; set; }
    public LogLevel Level { get; set; }
    public string FileName { get; set; }
    public string Message { get; set; }

    public LogEntry(LogLevel level, string message, string filename = "")
    {
       this.Timestamp = DateTime.Now;
       this.Level = level;
       this.Message = message;
       this.FileName = filename;
    }
}

