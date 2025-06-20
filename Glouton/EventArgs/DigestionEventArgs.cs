using System;

namespace Glouton.EventArgs;

public class DigestionEventArgs : System.EventArgs
{
    public GloutonAppreciation Tasting { get; }

    public DigestionEventArgs(GloutonAppreciation tasting)
    {
        Tasting = tasting;
    }
}

public enum GloutonAppreciation
{
    Ok = 0,
    Wonderful = 1,
    Awful = 2
}