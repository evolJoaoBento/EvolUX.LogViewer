using Microsoft.Extensions.DependencyInjection;
using EvolUX.LogViewer.Services;
using EvolUX.LogViewer.ViewModels;
using System.Windows;

namespace EvolUX.LogViewer
{
    public partial class App : System.Windows.Application
    {
        private ServiceProvider serviceProvider;

        public App()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            serviceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(ServiceCollection services)
        {
            // Register services
            services.AddSingleton<ILogParserService, LogParserService>();
            services.AddSingleton<ILogSearchService, LogSearchService>();
            services.AddSingleton<ITimeZoneService, TimeZoneService>();

            // Register view models
            services.AddTransient<MainViewModel>();
            services.AddTransient<LogListViewModel>();
            services.AddTransient<LogDetailViewModel>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var mainWindow = new MainWindow
            {
                DataContext = serviceProvider.GetService<MainViewModel>()
            };

            mainWindow.Show();
        }

        // Add a service provider accessor
        public static ServiceProvider Services => ((App)Current).serviceProvider;
    }
}