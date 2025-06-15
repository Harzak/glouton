using System;
using System.IO;
using System.Threading;

namespace Glouton.Interfaces;

public interface IFileEventDispatcher : IDisposable
{
    void BeginInvoke(FileSystemEventArgs args, Action action, CancellationToken cancellationToken = default);
}