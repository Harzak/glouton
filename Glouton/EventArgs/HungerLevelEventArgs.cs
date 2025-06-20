using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glouton.EventArgs;

public class HungerLevelEventArgs : System.EventArgs
{
    public int HungerLevel { get; }

    public HungerLevelEventArgs(int hungerLevel)
    {
        HungerLevel = hungerLevel;
    }
}
