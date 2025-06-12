namespace Glouton.Settings;

public class AppSettings
{
    public string WatchedFilePath { get; set; } = string.Empty;
    public int BatchExecutionInterval { get; set; } = 500;
    public int MaxBatchItem { get; set; } = 50; 
}