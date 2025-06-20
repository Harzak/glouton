using Glouton.EventArgs;
using Glouton.Utils.Time;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using Threading = System.Threading;

namespace Glouton.Features.FileManagement.FileDetection;

internal class FileScan : IDisposable
{
    private Threading.Timer? _timer;
    private  readonly string _location;
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

    public void Start()
    {
        if(string.IsNullOrEmpty(_location) || !Directory.Exists(_location))
        {
            throw new ArgumentException("Invalid directory location specified.", nameof(_location));
        }

        _timer = new Threading.Timer(this.OnTimerCallback, state: null, (int)_defaultPolicy.DueTime.TotalMilliseconds, (int)_defaultPolicy.Period.TotalMilliseconds);
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

public class  ScanPolicy
{
    public required TimeSpan DueTime { get; set; }
    public required TimeSpan Period { get; set; }
    public required TimeSpan Duration { get; set; }

    public static ScanPolicy SlowScanPolicy => new ScanPolicy
    {
        DueTime = TimeSpan.FromMilliseconds(300),
        Period = TimeSpan.FromMinutes(2),
        Duration = TimeSpan.MinValue
    };

    public static ScanPolicy FastScanPolicy => new ScanPolicy
    {
        DueTime = TimeSpan.FromMilliseconds(300),
        Period = TimeSpan.FromMilliseconds(300),
        Duration = TimeSpan.FromSeconds(5)
    };
}