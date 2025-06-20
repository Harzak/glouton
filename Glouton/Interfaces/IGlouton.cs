using Glouton.EventArgs;
using System;

namespace Glouton.Interfaces;

public interface IGlouton : IDisposable
{
    int HungerLevel { get; set; }

    event EventHandler<HungerLevelEventArgs>? HungerLevelChanged;

    void WakeUp();
}