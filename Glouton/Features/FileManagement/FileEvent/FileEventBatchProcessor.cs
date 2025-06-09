using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Timers;
using Glouton.Interfaces;
using Glouton.Utils.Time;

namespace Glouton.Features.FileManagement.FileEvent;

/// <summary>
/// Processes events triggered by the file watcher in batch mode
/// </summary>
internal sealed class FileEventBatchProcessor : IDisposable
{
    private readonly ILoggingService _logger;   

    private readonly int _maxBatchItem;
    private readonly ConcurrentQueue<FileEventActionModel> _queue;
    private readonly ConcurrentTimer _timer;
    private readonly Action<List<FileEventActionModel>> _filesAction;

    internal FileEventBatchProcessor(Action<List<FileEventActionModel>> filesAction,
        ILoggingService logger,
        int batchExecutionInterval,
        int maxBatchItem)
    {
        _filesAction = filesAction;
        _logger = logger;
        _maxBatchItem = maxBatchItem;
        _queue = [];
        _timer = new ConcurrentTimer(batchExecutionInterval)
        {
            RunUntil = () => _queue.IsEmpty
        };
        _timer.Elapsed += OnTimerElapsed;
    }

    public void Enqueue(FileEventActionModel model)
    {
        _queue.Enqueue(model);
        _timer.Start();
        _logger.LogDebug($"The file is in queue.", model.FileName);
    }

    private void OnTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        List<FileEventActionModel> list = [];
        int index = 0;
        while (index < _maxBatchItem && _queue.TryDequeue(out var elem))
        {
            list.Add(elem);
            index++;
        }

        _filesAction.Invoke(list);
    }   

    public void Dispose()
    {
        _timer?.Dispose();
    }
}

