using Glouton.Interfaces;
using Glouton.Settings;
using Microsoft.Extensions.Options;
using System;
using System.Timers;

namespace Glouton.Utils.Time;

/// <summary>
/// Thread-safe timer with end-action
/// </summary>
internal sealed class ConcurrentTimer : ITimer
{
    private readonly object _lock;
    private bool _isStarted;
    private readonly Timer _timer;

    public event ElapsedEventHandler? Elapsed;

    public Func<bool>? RunUntil { get; set; }

    public ConcurrentTimer(IOptions<BatchTimerSettings> options)
    {
        _lock = new object();
        _timer = new Timer(options.Value.IntervalMs)
        {
            AutoReset = options.Value.AutoReset
        };
        _timer.Elapsed += OnTimerElapsed;
    }

    public ITimer StartTimer()
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

    public void StopTimer()
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
            this.StopTimer();
            return;
        }

        this.Elapsed?.Invoke(this, e);
    }

    public void Dispose()
    {
        StopTimer();
        if (_timer != null)
        {
            _timer.Elapsed -= OnTimerElapsed;
            _timer.Dispose();
        }
    }
}