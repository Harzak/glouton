using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glouton.Features.FileManagement.FileDetection;

internal class FileTrackingInfo
{
    private static readonly TimeSpan DispatchThrottleWindow = TimeSpan.FromSeconds(1);

    public FileInfoSnapshot FileInfo { get; set; }
    public required DateTime LastSeenUtc { get; set; }
    public DateTime? LastDispatchedUtc { get; set; }

    public FileTrackingInfo(FileInfoSnapshot fileInfo)
    {
        FileInfo = fileInfo;
    }

    public void UpdateFileInfo(FileInfoSnapshot newFileInfo, DateTime seenUtc)
    {
        FileInfo = newFileInfo;
        LastSeenUtc = seenUtc;
        LastDispatchedUtc = null;
    }

    public bool WasDispatchedRecently(DateTime now)
    {
        return LastDispatchedUtc.HasValue &&
               (now - LastDispatchedUtc.Value) < DispatchThrottleWindow;
    }
}