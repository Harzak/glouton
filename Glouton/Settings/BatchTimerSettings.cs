namespace Glouton.Settings;

internal class BatchTimerSettings
{
    public int IntervalMs { get; set; } = 50;
    public bool AutoReset { get; set; } = true;
}