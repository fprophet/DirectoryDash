using DirectoryDash.Helpers;
using DirectoryDash.Services;
using DirectoryDash.Stores;
using DirectoryDash.ViewModels;
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
            MainWindow window = new MainWindow();
            var iconService = new Services.IconService();
            var explorerService = new Services.ExplorerService();
            var store = new ContainersStore();
            window.DataContext = new MainViewModel(explorerService, iconService, store);
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
