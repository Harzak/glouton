using Glouton.Interfaces;
using System;

namespace Glouton.Utils.RetryPolicies;

internal class ExponentialRetry : IRetryPolicy
{
    private const int BASE_DELAY = 30;

    public int MaxAttemps => 30;

    public TimeSpan GetNextRetryDelay(int attempt)
    {
        if (attempt < this.MaxAttemps)
        {
            return TimeSpan.FromMilliseconds(BASE_DELAY * Math.Pow(2, attempt));
        }
        else
        {
            return TimeSpan.Zero;
        }
    }
}

