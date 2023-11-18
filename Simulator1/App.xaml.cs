using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Simulator1.HostBuilder;
using Simulator1.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Simulator1
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IHost _host;

        public App()
        {
            _host = CreateHostBuilder().Build();
        }

        public static IHostBuilder CreateHostBuilder(string[] args = null)
        {
            return Host.CreateDefaultBuilder(args)
                        .AddServices()
                        .AddViewModels()
                        .AddViews();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
                _host.Start();
                MainViewModel mainViewModel = _host.Services.GetRequiredService<MainViewModel>();
                MainWindow window = _host.Services.GetRequiredService<MainWindow>();
                window.DataContext = mainViewModel;
                window.Show();
            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await _host.StopAsync();
            _host.Dispose();

            base.OnExit(e);
        }
    }
}
