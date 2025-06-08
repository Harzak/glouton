using System;
using System.IO;
using System.Threading;

namespace Glouton.Interfaces;

public interface IFileEventDispatcher
{
    void BeginInvoke(FileSystemEventArgs args, Action action, CancellationToken cancellationToken = default);
}