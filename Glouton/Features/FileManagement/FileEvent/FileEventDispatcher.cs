using Glouton.Interfaces;
using Glouton.Settings;
using Glouton.Utils.TaskScheduling;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Glouton.Features.FileManagement.FileEvent;

internal sealed class FileEventDispatcher : IFileEventDispatcher
{
    private readonly ILoggingService _logger;

    private bool _disposedValue;
    private readonly IFileEventBatchProcessor _batchProcessor;

    public FileEventDispatcher(IFileEventBatchProcessor batchProcessor, ILoggingService logger)
    {
        _batchProcessor = batchProcessor;
        _logger = logger;

        _batchProcessor.Initialize(Invoke);
    }

    public void BeginInvoke(FileSystemEventArgs args, Action action, CancellationToken cancellationToken = default)
    {
        FileEventActionModel actionInvoker = new(cancellationToken)
        {
            Action = action,
            Id = Guid.NewGuid(),
            EventArgs = args
        };
        this.BeginInvokeInner(() => _batchProcessor.Enqueue(actionInvoker), cancellationToken);
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
            LogAction(actions.First());
        }
        else
        {
            for (var index = 0; index < actions.Count - 1; index++)
            {
                this.Invoke(actions[index], SingleTaskScheduler.Current);
                LogAction(actions[index]);
            }
        }

        void LogAction(FileEventActionModel action) => _logger.LogInfo($"The file has been processed.", action.FileName);
    }

    private Task Invoke(FileEventActionModel model, TaskScheduler taskScheduler)
    {
        return Task.Factory.StartNew(model.Action, model.CancellationToken, TaskCreationOptions.None, taskScheduler);
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
                _batchProcessor?.Dispose();
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