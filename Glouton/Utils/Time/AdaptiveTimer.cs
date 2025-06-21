using System;
using System.Timers;

namespace Glouton.Utils.Time;

internal class AdaptiveTimer : IDisposable
{
    private readonly Timer _systemTimer;

    private Action<object?, ElapsedEventArgs>? _callback;
    private Func<bool>? _fastIntervalCondition;

    private readonly double _slowInterval;
    private readonly double _fastInterval;
    public bool IsInFastMode { get; private set; }

    public AdaptiveTimer(TimeSpan slowInterval, TimeSpan fastInterval, bool autoReset = true)
    {
        _slowInterval = slowInterval.TotalMilliseconds;
        _fastInterval = fastInterval.TotalMilliseconds;
        _systemTimer = new Timer()
        {
            Interval = _slowInterval,
            AutoReset = autoReset
        };
        _systemTimer.Elapsed += OnSystemTimerElapsed;
    }

    private void OnSystemTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        _callback?.Invoke(this, e);
        Adapt();
    }

    public void Adapt()
    {
        bool shouldBeFast = _fastIntervalCondition?.Invoke() == true;

        if (IsInFastMode != shouldBeFast)
        {
            ChangeTimerInterval(shouldBeFast ? _fastInterval : _slowInterval);
            IsInFastMode = shouldBeFast;
        }
    }

    private void ChangeTimerInterval(double newInterval)
    {
        _systemTimer.Stop();
        _systemTimer.Interval = newInterval;
        _systemTimer.Start();
    }

    public void StartTimer()
    {
        _systemTimer.Start();
    }

    public void StopTimer()
    {
        _systemTimer.Stop();
    }

    public AdaptiveTimer WithCallback(Action<object?, ElapsedEventArgs> action)
    {
        ArgumentNullException.ThrowIfNull(action);

        _callback = action;
        return this;
    }

    public AdaptiveTimer FastIntervalWhen(Func<bool> condition)
    {
        ArgumentNullException.ThrowIfNull(condition);
        _fastIntervalCondition = condition;
        return this;
    }

    public void Dispose()
    {
        this.StopTimer();
        _systemTimer.Elapsed -= OnSystemTimerElapsed;
        _systemTimer?.Dispose();
        _callback = null;
        _fastIntervalCondition = null;
    }
}