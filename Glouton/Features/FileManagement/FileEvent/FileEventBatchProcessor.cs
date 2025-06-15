using Glouton.Interfaces;
using Glouton.Settings;
using Glouton.Utils.Time;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Timers;

[assembly: InternalsVisibleTo("Glouton.Tests")]

namespace Glouton.Features.FileManagement.FileEvent;

/// <summary>
/// Processes events triggered by the file watcher in batch mode
/// </summary>
internal sealed class FileEventBatchProcessor : IFileEventBatchProcessor
{
    private readonly ILoggingService _logger;

    private readonly int _maxBatchItem;
    private readonly ConcurrentQueue<FileEventActionModel> _queue;
    private readonly ITimer _timer;

    private Action<List<FileEventActionModel>>? _filesAction;

    public FileEventBatchProcessor(IOptions<BatchSettings> options,ILoggingService logger, ITimer timer)
    {
        _maxBatchItem = options.Value.MaxItems;
        _logger = logger;
        _timer = timer;
        _queue = [];
    }

    public void Initialize(Action<List<FileEventActionModel>> filesAction)
    {
        _filesAction = filesAction;
        _timer.RunUntil = () => _queue.IsEmpty;
        _timer.Elapsed += OnTimerElapsed;
    }

    public void Enqueue(FileEventActionModel model)
    {
        if(_filesAction is null)
        {
            throw new InvalidOperationException("The batch processor has not been initialized. Call Initialize first.");
        }

        _queue.Enqueue(model);
        _timer.StartTimer();
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

        _filesAction?.Invoke(list);
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}

