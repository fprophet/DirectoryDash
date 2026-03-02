using DirectoryDash.Factories;
using DirectoryDash.Helpers;
using DirectoryDash.Models;
using DirectoryDash.Services;
using DirectoryDash.Stores;
using DirectoryDash.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;

namespace DirectoryDash
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {

            SettingsHelper.CheckSettings();

            ServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<ExplorerService>();
            serviceCollection.AddSingleton<IconService>();
            serviceCollection.AddSingleton<ContainersStore>();

            serviceCollection.AddTransient<MainWindow>();
            serviceCollection.AddTransient<MainViewModel>();
            serviceCollection.AddTransient<ContainerViewModel>();
            serviceCollection.AddTransient<SettingsViewModel>();
            serviceCollection.AddTransient<ItemListViewModel>();
            serviceCollection.AddTransient<ItemFactory>();


            serviceCollection.AddTransient<Func<ExplorerContainerData, ContainerViewModel>>(s => data =>
                ActivatorUtilities.CreateInstance<ContainerViewModel>(s, data));

            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

            var window = serviceProvider.GetRequiredService<MainWindow>();
            var mainViewModel = serviceProvider.GetRequiredService<MainViewModel>();
            
            window.DataContext = mainViewModel;
            window.Show();


            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            //_icon?.Dispose();
            base.OnExit(e);
        }
    }

}
