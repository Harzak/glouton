using System;

namespace Glouton.Interfaces;

public interface IGlouton : IDisposable
{
    int HungerLevel { get; set; }

    event EventHandler? HungerLevelChanged;

    void WakeUp();
}