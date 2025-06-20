using Glouton.EventArgs;
using System;
using System.IO;
using System.Threading;

namespace Glouton.Interfaces;

public interface IFileEventDispatcher : IDisposable
{
    void BeginInvoke(DetectedFileEventArgs args, Action action, CancellationToken cancellationToken = default);
}