using System;
using System.Timers;

namespace Glouton.Interfaces;

public interface ITimer : IDisposable
{
    event ElapsedEventHandler? Elapsed;

    Func<bool>? RunUntil { get; set; }  

    ITimer StartTimer();
    void StopTimer();
}