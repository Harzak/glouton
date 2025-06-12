using System;

namespace Glouton.Interfaces;

public interface IRetryPolicy
{
    int MaxAttemps { get; }

    TimeSpan GetNextRetryDelay(int attempt);
}

