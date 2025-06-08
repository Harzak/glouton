using Glouton.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Glouton.Extensions;

internal static class ViewModelsHostBuilderExtensions
{
    public static IHostBuilder BuildViewModels(this IHostBuilder hostBuilder)
    {
        hostBuilder.ConfigureServices(services =>
        {
            services.AddSingleton<MainWindowViewModel>();
        });

        return hostBuilder;
    }
}