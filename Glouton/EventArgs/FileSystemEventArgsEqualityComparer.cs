using System;
using System.Collections.Generic;
using System.IO;

namespace Glouton.EventArgs;

internal sealed class FileSystemEventArgsEqualityComparer : IEqualityComparer<FileSystemEventArgs>
{
    public bool Equals(FileSystemEventArgs? x, FileSystemEventArgs? y)
    {
        if (x == null && y == null)
        {
            return true;
        }
        if (x == null || y == null)
        {
            return false;
        }
        return x.FullPath.Equals(y.FullPath, StringComparison.OrdinalIgnoreCase) && x.ChangeType == y.ChangeType;
    }

    public int GetHashCode(FileSystemEventArgs obj)
    {
        return HashCode.Combine(obj.FullPath.ToUpperInvariant(), obj.ChangeType);
    }
}