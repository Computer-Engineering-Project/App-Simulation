using Environment.Service;
using Environment.Service.Interface;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Simulator1.State_Management;
using Simulator1.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Simulator1.HostBuilder
{
    public static class AddServicesHostBuilderExtension
    {
        public static IHostBuilder AddServices(this IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureServices(services =>
            {
                //State Management
                services.AddSingleton<ModuleStateManagement>();
                //Store
                services.AddSingleton<ModuleStore>();
                services.AddSingleton<NavigationStore>();
                services.AddSingleton<MainStore>();
                services.AddSingleton<ModuleParameterViewStore>();
                //Environment
                services.AddSingleton<IEnvironmentService, EnvironmentService>();
            });
            return hostBuilder;
        }
    }
}
