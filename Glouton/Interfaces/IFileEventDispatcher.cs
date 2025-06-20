using Glouton.EventArgs;
using System;
using System.Threading;

namespace Glouton.Interfaces;

public interface IFileEventDispatcher : IDisposable
{
    void BeginInvoke(DetectedFileEventArgs args, Action action, CancellationToken cancellationToken = default);
}