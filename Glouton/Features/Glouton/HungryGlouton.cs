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

    private Stomach _stomach;

    public int HungerLevel { get; set; }

    public event EventHandler? HungerLevelChanged;

    public HungryGlouton(IFileWatcherService watcher,
        IFileSystemDeletionFactory deletionFactory,
        ILoggingService logger)
    {
        _watcher = watcher;
        _deletionFactory = deletionFactory;
        _logger = logger;
        _stomach = new Stomach();
        HungerLevel = 50;
    }

    public void WakeUp()
    {
        _watcher.StartWatcher(@"C:\Users\Aurelien\Desktop\glouton");
        _watcher.FileChanged += OnFileCreated;
        _stomach.FoodDigested +=OnFoodDigested;
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
            _stomach.AddFood(new Food()
            {
                Extension = Path.GetExtension(path),
                FullPath = path
            });
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

    private void OnFoodDigested(object? sender, DigestionEventArgs e)
    {
        switch (e.Tasting)
        {
            case GloutonAppreciation.Wonderful:
                HungerLevel += 10;
                break;
            case GloutonAppreciation.Awful:
                HungerLevel -= 10;
                break;
            default:
                break;
        }
        HungerLevelChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Dispose()
    {
        if (_watcher != null)
        {
            _watcher.FileChanged -= OnFileCreated;
        }
    }
}

