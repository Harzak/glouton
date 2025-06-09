using Glouton.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glouton.Utils.RetryPolicies;

internal class FixedRetry : IRetryPolicy
{
    private readonly TimeSpan _delay;

    public int MaxAttemps => 20;

    public FixedRetry(TimeSpan timeToWait)
    {
        _delay = timeToWait;
    }

    public TimeSpan GetNextRetryDelay(int attempt)
    {
        return attempt < this.MaxAttemps ? _delay : TimeSpan.Zero; 
    }
}