using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Simulator1.View;
using Simulator1.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator1.HostBuilder
{
    public static class AddViewsHostBuilderExtension
    {
        public static IHostBuilder AddViews(this IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureServices(services =>
            {
                services.AddSingleton<MainWindow>(s => new MainWindow()
                {
                    DataContext = s.GetRequiredService<MainViewModel>()
                });
                
            });
            return hostBuilder;
        }
    }
}
