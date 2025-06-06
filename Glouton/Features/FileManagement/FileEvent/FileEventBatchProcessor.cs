using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Timers;
using Glouton.Utils.Time;

namespace Glouton.Features.FileManagement.FileEvent;

internal sealed class FileEventBatchProcessor : IDisposable
{
    private readonly int _maxBatchItem;
    private readonly ConcurrentQueue<FileEventActionModel> _queue;
    private readonly ConcurrentTimer _timer;
    private readonly Action<List<FileEventActionModel>> _action;

    internal FileEventBatchProcessor(Action<List<FileEventActionModel>> action, 
        int batchExecutionInterval,
        int maxBatchItem)
    {
        _action = action;
        _maxBatchItem = maxBatchItem;
        _queue = [];
        _timer = new ConcurrentTimer(batchExecutionInterval)
        {
            RunUntil = () => _queue.IsEmpty
        };
        _timer.Elapsed += OnTimerElapsed;
    }

    private void OnTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        var list = new List<FileEventActionModel>();
        int index = 0;
        while (index < _maxBatchItem && _queue.TryDequeue(out var elem))
        {
            list.Add(elem);
            index++;
        }

        _action.Invoke(list);
    }   

    public void Enqueue(FileEventActionModel model)
    {
        _queue.Enqueue(model);
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}

