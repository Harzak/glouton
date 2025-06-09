using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glouton.Interfaces;

public interface IRetryPolicy
{
    int MaxAttemps { get; }

    TimeSpan GetNextRetryDelay(int attempt);
}

