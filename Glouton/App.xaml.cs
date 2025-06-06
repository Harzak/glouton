using Glouton.Extensions;
using Glouton.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Configuration;
using System.Data;
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
                    DataContext = s.GetRequiredService<HomeViewModel>()
                });
                //services.AddSingleton<IThemeService, ThemeService>();
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