using System;

namespace Glouton.EventArgs;

public class DetectedFileEventArgs : System.EventArgs
{
    public string FilePath { get; set; }
    public DateTime DetectedAtUtc { get; set; }

    public DetectedFileEventArgs(string filePath, DateTime detectedAt)
    {
        this.FilePath = filePath;
        this.DetectedAtUtc = detectedAt;
    }
}

