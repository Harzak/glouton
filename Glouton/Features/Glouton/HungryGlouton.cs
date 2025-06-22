using Glouton.EventArgs;
using Glouton.Interfaces;
using Glouton.Utils.Result;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Glouton.Features.Glouton;

internal sealed class HungryGlouton : IGlouton
{
    public const int DEFAULT_HUNGER_LEVEL = 50;

    private readonly IFileDetection _detection;
    private readonly ISettingsService _settingsService;
    private readonly ILoggingService _logger;
    private readonly IFileSystemDeletion _deletion;

    private readonly Stomach _stomach;

    public int HungerLevel { get; set; }

    public event EventHandler<HungerLevelEventArgs>? HungerLevelChanged;

    public HungryGlouton(IFileDetection detection,
        IFileSystemDeletionFactory deletionFactory,
        ISettingsService settingsService,
        ILoggingService logger)
    {
        _detection = detection;
        _settingsService = settingsService;
        _logger = logger;
        _deletion = deletionFactory.CreateDeletionWithExponentialRetry();
        _stomach = new Stomach();
        this.HungerLevel = DEFAULT_HUNGER_LEVEL;
    }

    public void WakeUp()
    {
        _detection.StartDetection(_settingsService.GetSettings().WatchedFilePath);
        _detection.InvokeOnFileDetected(OnFileDetected);
        _stomach.FoodDigested += OnFoodDigested;
    }

    private void OnFileDetected(DetectedFileEventArgs e)
    {
        _ =  this.EatAsync(e.FilePath);
    }

    private async Task EatAsync(string path)
    {

        OperationResult result = await _deletion.StartAsync(path).ConfigureAwait(false);
        if (result.IsSuccess)
        {
            _logger.LogInfo($"Glouton ate the file.", Path.GetFileName(path));
            _stomach.AddFood(new Food()
            {
                Extension = Path.GetExtension(path),
                FullPath = path
            });
        }
        else
        {
            _logger.LogError($"Glouton did not like the file.", Path.GetFileName(path));
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
        HungerLevelChanged?.Invoke(this, new HungerLevelEventArgs(HungerLevel));
    }

    public void Dispose()
    {
        _detection?.Dispose();
        if (_stomach != null)
        {
            _stomach.FoodDigested -= OnFoodDigested;
        }
    }
}