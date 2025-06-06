using Glouton.Utils.TaskScheduling;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Glouton.Features.FileManagement.FileEvent;

internal sealed class FileEventDispatcher : IDisposable
{
    private const int BATCH_EXECUTION_INTERVAL = 500;
    private const int MAX_BATCH_ITEM = 50;

    private bool _disposedValue;
    private static FileEventDispatcher? _inner;
    private readonly FileEventBatchProcessor _actionQueue;

    public static FileEventDispatcher Current => _inner ??= new FileEventDispatcher();

    private FileEventDispatcher()
    {
        _actionQueue = new FileEventBatchProcessor(action: Invoke, BATCH_EXECUTION_INTERVAL, MAX_BATCH_ITEM);
    }

    private void Invoke(List<FileEventActionModel> actions)
    {
        if (actions is null || actions.Count == 0)
        {
            return;
        }

        if (actions.Count == 1)
        {
            this.Invoke(actions.First(), ParallelTaskScheduler.Current);
        }
        else
        {
            for (var index = 0; index < actions.Count - 1; index++)
            {
                this.Invoke(actions[index], SingleTaskScheduler.Current);
            }
        }
    }

    private Task Invoke(FileEventActionModel model, TaskScheduler taskScheduler)
    {
        return Task.Factory.StartNew(model.Action, model.CancellationToken, TaskCreationOptions.None, taskScheduler);
    }

    public void BeginInvoke(FileSystemEventArgs args, Action action, CancellationToken cancellationToken = default)
    {
        FileEventActionModel actionInvoker = new(id: Guid.NewGuid(), action, cancellationToken)
        {
            EventArgs = args
        };
        this.BeginInvokeInner(() => _actionQueue.Enqueue(actionInvoker), cancellationToken);
    }

    private void BeginInvokeInner(Action action, CancellationToken cancellationToken = default)
    {
        Task.Run(action, cancellationToken);
    }

    private void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _actionQueue?.Dispose();
            }

            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}