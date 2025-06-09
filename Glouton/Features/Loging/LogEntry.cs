using Microsoft.Extensions.Logging;
using System;

namespace Glouton.Features.Loging;

public record LogEntry
{
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public required LogLevel Level { get; set; }
    public string FileName { get; set; } = string.Empty;
    public required string Message { get; set; }
}