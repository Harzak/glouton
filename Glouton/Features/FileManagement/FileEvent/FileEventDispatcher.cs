using Glouton.EventArgs;
using Glouton.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Glouton.Features.FileManagement.FileEvent;

/// <summary>
/// Bridge between file detection events and their handling.
/// <para>
/// Separates components that detect file events from the code that processes those events.
/// Handles queueing, batching, and asynchronous execution of file event actions.
/// </para>
/// </summary>
internal sealed class FileEventDispatcher : IFileEventDispatcher
{
    private bool _disposedValue;
    private readonly ILoggingService _logger;
    private readonly IFileEventBatchProcessor _batchProcessor;

    public FileEventDispatcher(IFileEventBatchProcessor batchProcessor, ILoggingService logger)
    {
        _batchProcessor = batchProcessor;
        _logger = logger;

        _batchProcessor.Initialize(Invoke);
    }

    public void Enqueue(DetectedFileEventArgs args, Action action, CancellationToken cancellationToken = default)
    {
        FileEventActionModel actionInvoker = new(cancellationToken)
        {
            Action = action,
            EventArgs = args
        };
        _batchProcessor.Enqueue(actionInvoker);
    }

    private void Invoke(List<FileEventActionModel> actions)
    {
        if (actions is null || actions.Count == 0)
        {
            return;
        }

        List<FileEventActionModel> validActions = actions.Where(a => !a.CancellationToken.IsCancellationRequested).ToList();

        if (validActions.Count <= 5)
        {
            foreach (FileEventActionModel action in validActions)
            {
                InvokeAction(action);
                LogAction(action);
            }
        }
        else
        {
            Parallel.ForEach(validActions, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, action =>
            {
                InvokeAction(action);
                LogAction(action);
            });
        }

        void LogAction(FileEventActionModel action) 
        { 
            _logger.LogInfo($"The file has been processed.", action.FileName); 
        }
    }

    private void InvokeAction(FileEventActionModel model)
    {
        Task.Factory.StartNew(
            model.Action,
            model.CancellationToken,
            TaskCreationOptions.DenyChildAttach,
            TaskScheduler.Default)
        .ContinueWith(t =>
        {
            if (t.IsFaulted && t.Exception != null)
            {
                _logger.LogError($"Action failed: {t.Exception.InnerException?.Message}", model.FileName);
            }
        }, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
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