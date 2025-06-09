using System;

namespace Glouton.Features.Glouton;

public class DigestionEventArgs : EventArgs
{
    public GloutonAppreciation Tasting { get; }

    public DigestionEventArgs(GloutonAppreciation tasting)
    {
        this.Tasting = tasting;
    }
}

public enum GloutonAppreciation
{
    Ok = 0,
    Wonderful = 1,
    Awful = 2
}