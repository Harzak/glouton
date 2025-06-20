using Glouton.Features.FileManagement.FileDeletion;
using Glouton.Features.FileManagement.FileDetection;
using Glouton.Features.FileManagement.FileEvent;
using Glouton.Features.Glouton;
using Glouton.Features.Loging;
using Glouton.Features.Menu;
using Glouton.Interfaces;
using Glouton.Settings;
using Glouton.Utils.Time;
using Glouton.ViewModels;
using Glouton.Views;
using Glouton.Wrappers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Glouton.Extensions;

internal static class IServiceCollectionExtensions
{
    public static void ConfigureSettings(this IServiceCollection services, HostBuilderContext hostContext)
    {
        services.Configure<BatchTimerSettings>(hostContext.Configuration.GetSection("BatchTimerSettings"));
        services.Configure<BatchSettings>(hostContext.Configuration.GetSection("BatchSettings"));
    }

    public static void RegisterViewModels(this IServiceCollection services)
    {
        services.AddSingleton<MainWindowViewModel>();
        services.AddSingleton(s => new MainWindow()
        {
            DataContext = s.GetRequiredService<MainWindowViewModel>()
        });
        services.AddSingleton<MainWindowViewModel>();
        services.AddSingleton<MenuViewModel>();
        services.AddSingleton<FileDetectionControlsViewModel>();
        services.AddSingleton<LogViewModel>();
        services.AddSingleton<GloutonViewModel>();

    }

    public static void RegisterAppServices(this IServiceCollection services)
    {
        services.AddSingleton<ISettingsService, SettingsService>();
        services.AddSingleton<IFileEventBatchProcessor, FileEventBatchProcessor>();
        services.AddSingleton<IFileEventDispatcher, FileEventDispatcher>();
        services.AddSingleton<IFileSystemDeletionFactory, FileSystemDeletionFactory>();
        services.AddSingleton<IFileDetection, FileDetectionCoordinator>();
        services.AddSingleton<ILoggingService, AppLogger>();
        services.AddSingleton<IGlouton, HungryGlouton>();
    }

    public static void RegisterUtilities(this IServiceCollection services)
    {
        services.AddSingleton<ITimer, ConcurrentTimer>();
        services.AddSingleton<IMenuCommandInvoker, MenuCommandInvoker>();
        services.AddSingleton<IDirectoryFacade, DirectoryFacade>();
        services.AddSingleton<IFileSystemFacade, FileFacade>();
        services.AddSingleton<IProcessFacade, ProcessFacade>();
    }
}