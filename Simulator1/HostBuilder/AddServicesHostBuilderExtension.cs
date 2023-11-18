using Environment.Base;
using Environment.Service;
using Environment.Service.Interface;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Simulator1.Database;
using Simulator1.State_Management;
using Simulator1.Store;
using Simulator1.ViewModel;
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
                services.AddSingleton<MainStateManagement>();
                //Store
                services.AddSingleton<ModuleStore>();
                services.AddSingleton<NavigationStore>();
                services.AddSingleton<MainStore>();
                services.AddSingleton<ModuleParameterViewStore>();
                //Database
                services.AddSingleton<LoadParameter>();
                //Environment
                services.AddSingleton<IEnvironmentService, EnvironmentService>();
                services.AddSingleton<ICommunication, MainViewModel>();

                services.AddSingleton<BaseEnvironment>();
            });
            return hostBuilder;
        }
    }
}
