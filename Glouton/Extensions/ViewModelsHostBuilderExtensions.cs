using Glouton.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glouton.Extensions;

internal static class ViewModelsHostBuilderExtensions
{
    public static IHostBuilder BuildViewModels(this IHostBuilder hostBuilder)
    {
        hostBuilder.ConfigureServices(services =>
        {
            services.AddSingleton<HomeViewModel>();
        });

        return hostBuilder;
    }
}