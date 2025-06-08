using System;
using System.Timers;

namespace Glouton.Utils.Time;

internal sealed class ConcurrentTimer : IDisposable
{
    private readonly object _lock;
    private bool _isStarted;

    private readonly Timer _timer;

    public event ElapsedEventHandler? Elapsed;
    public Func<bool>? RunUntil;

    internal ConcurrentTimer(double interval, bool autoReset = true)
    {
        _lock = new object();
        _timer = new Timer(interval)
        {
            AutoReset = autoReset
        };
        _timer.Elapsed += OnTimerElapsed;
    }

    public ConcurrentTimer Start()
    {
        lock (_lock)
        {
            if (!_isStarted)
            {
                _timer?.Start();
                _isStarted = true;
            }
        }
        return this;
    }

    public void Stop()
    {
        lock (_lock)
        {
            if (_isStarted)
            {
                _timer?.Stop();
                _isStarted = false;
            }
        }
    }

    private void OnTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        if (this.RunUntil != null && this.RunUntil.Invoke())
        {
            this.Stop();
            return;
        }

        this.Elapsed?.Invoke(this, e);
    }

    public void Dispose()
    {
        Stop();
        _timer?.Dispose();
    }
}