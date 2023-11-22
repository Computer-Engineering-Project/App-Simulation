using Environment.Service;
using Environment.Service.Interface;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Prism.Regions;
using Simulator1.Service;
using Simulator1.Service.Implement;
using Simulator1.State_Management;
using Simulator1.Store;
using Simulator1.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace Simulator1.HostBuilder
{
    public static class AddViewModelsHostBuilderExtension
    {
        public static IHostBuilder AddViewModels(this IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureServices(services =>
            {
                services.AddTransient<ZigbeeParameterViewModel>();
                services.AddTransient<testModuleViewModel>();
                services.AddTransient<LoraParameterViewModel>();
                services.AddTransient<ModuleParameterViewModel>((serviceProvider) =>
                {
                    var moduleParamStore = serviceProvider.GetRequiredService<ModuleParameterViewStore>();
                    moduleParamStore.CurrentViewModel = serviceProvider.GetRequiredService<LoraParameterViewModel>();
                    return new ModuleParameterViewModel(moduleParamStore, serviceProvider.GetRequiredService<ModuleStateManagement>(), serviceProvider.GetRequiredService<ModuleStore>(),
                        serviceProvider, serviceProvider.GetRequiredService<HistoryDataStore>(),
                        CreateLoraParamNavigateService(serviceProvider, moduleParamStore), CreateZigbeeParamNavigateService(serviceProvider, moduleParamStore));
                });
                services.AddSingleton<MainViewModel>((serviceProvider) =>
                {
                    var mainStore = serviceProvider.GetRequiredService<MainViewStore>();
                    mainStore.CurrentViewModel = serviceProvider.GetRequiredService<ModuleParameterViewModel>();
                    return new MainViewModel(mainStore, serviceProvider.GetRequiredService<MainStateManagement>(), serviceProvider.GetRequiredService<ModuleStateManagement>(),
                        serviceProvider.GetRequiredService<ModuleStore>(), serviceProvider, serviceProvider.GetRequiredService<testModuleViewModel>(), serviceProvider.GetRequiredService<HistoryDataStore>());
                });
            });
            return hostBuilder;
        }
        private static INavigateService CreateLoraParamNavigateService(IServiceProvider serviceProvider, NavigationStore store)
        {
            return new NavigateService<LoraParameterViewModel>(store, () => serviceProvider.GetRequiredService<LoraParameterViewModel>());
        }
        private static INavigateService CreateZigbeeParamNavigateService(IServiceProvider serviceProvider, NavigationStore store)
        {
            return new NavigateService<ZigbeeParameterViewModel>(store, () => serviceProvider.GetRequiredService<ZigbeeParameterViewModel>());
        }
    }
}
