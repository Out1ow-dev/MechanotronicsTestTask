using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using MechanotronicsApp.Services;
using MechanotronicsApp.Data;
using MechanotronicsApp.Interfaces;

namespace MechanotronicsApp
{
    public partial class App : Application
    {
        private ServiceProvider? _serviceProvider;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();

            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ILoggingService, LoggingService>();
            services.AddSingleton<IDatabaseService, DatabaseService>();
            services.AddSingleton<DataStore>();
            services.AddSingleton<IDataStore>(provider => provider.GetRequiredService<DataStore>());
            services.AddSingleton<IDataGeneratorService, DataGeneratorService>();
            services.AddTransient<MainWindow>();
            services.AddTransient<DetailsWindow>();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (_serviceProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }

            base.OnExit(e);
        }
    }
} 