using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Glouton.Utils.TaskScheduling;

internal abstract class BaseTaskScheduler : TaskScheduler, IDisposable
{
    private readonly int _maxConcurrencyLevel;
    private readonly Thread _mainThread;
    private readonly BlockingCollection<Task> _tasks;

    public override int MaximumConcurrencyLevel => _maxConcurrencyLevel;

    protected BaseTaskScheduler(int maxConcurrencyLevel)
    {
        _maxConcurrencyLevel = maxConcurrencyLevel;
        _tasks = [];
        _mainThread = new Thread(new ThreadStart(this.Execute))
        {
            Name = GetType().Name
        };
        _mainThread.Start();
    }

    protected override void QueueTask(Task task)
    {
        _tasks.Add(task);
    }

    private void Execute()
    {
        foreach (Task task in _tasks.GetConsumingEnumerable())
        {
            this.TryExecuteTask(task);
        }
    }

    protected override IEnumerable<Task> GetScheduledTasks()
    {
        return _tasks.ToArray();
    }

    protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
    {
        return false;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _tasks?.CompleteAdding();
            _mainThread?.Join();

            _tasks?.Dispose();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}