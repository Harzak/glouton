namespace Glouton.EventArgs;

public class HungerLevelEventArgs : System.EventArgs
{
    public int HungerLevel { get; }

    public HungerLevelEventArgs(int hungerLevel)
    {
        HungerLevel = hungerLevel;
    }
}
