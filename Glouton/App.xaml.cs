using Glouton.Extensions;
using Glouton.Features.FileManagement.FileDeletion;
using Glouton.Features.FileManagement.FileEvent;
using Glouton.Features.FileManagement.FileWatcher;
using Glouton.Features.Glouton;
using Glouton.Features.Loging;
using Glouton.Features.Menu;
using Glouton.Interfaces;
using Glouton.Settings;
using Glouton.ViewModels;
using Glouton.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;

namespace Glouton;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private readonly IHost _host;

    public App()
    {
        _host = Host.CreateDefaultBuilder()
            .BuildViewModels()
            .ConfigureServices((hostContext, services) =>
            {
                services.AddSingleton(s => new MainWindow()
                {
                    DataContext = s.GetRequiredService<MainWindowViewModel>()
                });

                services.AddSingleton<ISettingsService, SettingsService>();
                services.AddSingleton<IMenuCommandInvoker, MenuCommandInvoker>();
                services.AddSingleton<IGlouton, HungryGlouton>();
                services.AddSingleton<IFileEventDispatcher, FileEventDispatcher>();
                services.AddSingleton<IFileWatcherService, FileWatcherService>();
                services.AddSingleton<ILoggingService, AppLogger>();
                services.AddSingleton<IFileSystemDeletionFactory, FileSystemDeletionFactory>();
            })
            .Build();
    }

    private async void OnStartup(object sender, StartupEventArgs e)
    {
        await _host.StartAsync().ConfigureAwait(false);

        MainWindow = _host.Services.GetRequiredService<MainWindow>();
        MainWindow.Show();
    }

    private async void OnExit(object sender, ExitEventArgs e)
    {
        await _host.StopAsync().ConfigureAwait(false);
        _host.Dispose();
    }
}