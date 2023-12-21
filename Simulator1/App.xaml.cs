using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Simulator1.HostBuilder;
using Simulator1.View.StatisticWindow;
using Simulator1.ViewModel;
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
            MainWindow mainwindow = _host.Services.GetRequiredService<MainWindow>();
            mainwindow.DataContext = mainViewModel;

            mainwindow.Show();
            ChartWindow chartWindow = _host.Services.GetRequiredService<ChartWindow>();
            ChartViewModel chartViewModel = _host.Services.GetRequiredService<ChartViewModel>();
            chartWindow.DataContext = chartViewModel;
            
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
