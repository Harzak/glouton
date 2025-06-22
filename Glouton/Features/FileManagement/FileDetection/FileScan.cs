using Glouton.EventArgs;
using Glouton.Utils.Result;
using System;
using System.IO;
using Threading = System.Threading;

namespace Glouton.Features.FileManagement.FileDetection;

/// <summary>
/// Performs periodic scans of a directory to detect all files within it.
/// Supports dynamic policy changes that affect the scan frequency and
/// automatically reverts to the default policy after a specified duration.
/// </summary>
internal class FileScan : IDisposable
{
    private Threading.Timer? _timer;
    private readonly string _location;
    private readonly ScanPolicy _defaultPolicy;
    private ScanPolicy? _requestedPolicy;
    private DateTime _lastPolicyChangeRequestTime;

    public event EventHandler<DetectedFileEventArgs>? FileDetected;

    public FileScan(string location, ScanPolicy policy)
    {
        _location = location;
        _defaultPolicy = policy;
        _lastPolicyChangeRequestTime = DateTime.MinValue;
    }

    public OperationResult Start()
    {
        if (string.IsNullOrEmpty(_location) || !Directory.Exists(_location))
        {
            return OperationResult.Error($"Invalid directory location specified: '{_location}'");
        }

        _timer = new Threading.Timer(this.OnTimerCallback, state: null, (int)_defaultPolicy.DueTime.TotalMilliseconds, (int)_defaultPolicy.Period.TotalMilliseconds);

        return OperationResult.Success;
    }

    public void Change(ScanPolicy policy)
    {
        _lastPolicyChangeRequestTime = DateTime.UtcNow;
        _requestedPolicy = policy;

        this.ChangeTimer(_requestedPolicy);
    }

    private void OnTimerCallback(object? state)
    {
        if (_requestedPolicy != null && DateTime.UtcNow - _lastPolicyChangeRequestTime >= _requestedPolicy.Duration)
        {
            this.ChangeTimer(_defaultPolicy);
            _requestedPolicy = null;
        }
        this.Scan();
    }

    private void ChangeTimer(ScanPolicy policy)
    {
        _timer?.Change((int)policy.DueTime.TotalMilliseconds, (int)policy.Period.TotalMilliseconds);
    }

    private void Scan()
    {
        string[] allFiles = Directory.GetFiles(_location, "*.*", SearchOption.TopDirectoryOnly);
        DateTime now = DateTime.UtcNow;
        foreach (var filePath in allFiles)
        {
            FileDetected?.Invoke(this, new DetectedFileEventArgs(filePath, now));
        }
    }

    public void Dispose()
    {
        _timer?.Dispose();
        _timer = null;
    }
}