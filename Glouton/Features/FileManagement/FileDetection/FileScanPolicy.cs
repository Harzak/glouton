using System;

namespace Glouton.Features.FileManagement.FileDetection;

public class ScanPolicy
{
    public required TimeSpan DueTime { get; set; }
    public required TimeSpan Period { get; set; }
    public required TimeSpan Duration { get; set; }

    public static ScanPolicy SlowScanPolicy => new ScanPolicy
    {
        DueTime = TimeSpan.FromMilliseconds(300),
        Period = TimeSpan.FromMinutes(2),
        Duration = TimeSpan.MinValue
    };

    public static ScanPolicy FastScanPolicy => new ScanPolicy
    {
        DueTime = TimeSpan.FromMilliseconds(300),
        Period = TimeSpan.FromMilliseconds(300),
        Duration = TimeSpan.FromSeconds(5)
    };
}

