using Glouton.Interfaces;
using Glouton.Utils.Result;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glouton.Features.Glouton;

internal sealed class HungryGlouton : IGlouton
{
    private IFileWatcherService _watcher;
    private IFileSystemDeletionFactory _deletionFactory;
    private ILoggingService _logger;

    public string? HungerLevel { get; set; }

    //public event EventHandler? HungerLevelChanged;

    public HungryGlouton(IFileWatcherService watcher,
        IFileSystemDeletionFactory deletionFactory,
        ILoggingService logger)
    {
        _watcher = watcher;
        _deletionFactory = deletionFactory;
        _logger = logger;
    }

    public void WakeUp()
    {
        _watcher.Start(@"C:\Users\Aurelien\Desktop\glouton");
        _watcher.Created += OnFileCreated;
    }

    private void OnFileCreated(object sender, FileSystemEventArgs e)
    {
        _ =  this.EatAsync(e.FullPath);
    }

    private async Task EatAsync(string path)
    {
        IFileSystemDeletion deletion = _deletionFactory.CreateDeletionWithExponentialRetry();
        OperationResult result = await deletion.StartAsync(path).ConfigureAwait(false);
        if (result.IsSuccess)
        {
            _logger.LogInfo($"Glouton ate the file: {path}");
        }
        else
        {
            _logger.LogError($"Glouton did not like the file: {path}. Error: {result.ErrorMessage}");
            if (result.HasError)
            {
                _logger.LogError(result.ErrorMessage);
            }
        }
    }

    public void Dispose()
    {
        if (_watcher != null)
        {
            _watcher.Created -= OnFileCreated;
        }
    }
}

