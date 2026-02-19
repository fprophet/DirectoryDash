using DirectoryDash.Helpers;
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
        private NotifyIcon _icon;
        protected override void OnStartup(StartupEventArgs e)
        {

            SettingsHelper.CheckSettings();
            MainWindow window = new MainWindow();
            window.DataContext = new MainViewModel(new Services.ExplorerService());
            window.Show();

            NotifyIcon _icon = new NotifyIcon();
            _icon.Icon = new Icon("tray.ico");
            _icon.Visible = true;
            _icon.Click += HandleClick;
            _icon.MouseClick += HandleClick;
            _icon.ContextMenuStrip = new ContextMenuStrip();
            _icon.ContextMenuStrip.Items.Add("Settings", null);
            _icon.ContextMenuStrip.Items.Add("-", null);
            _icon.ContextMenuStrip.Items.Add("Exit", null, (s,args) => System.Windows.Application.Current.Shutdown());
            _icon.ContextMenuStrip.Show();

            base.OnStartup(e);
        }

        private void HandleClick(object? sender, EventArgs e)
        {
            var icon = sender as NotifyIcon;
            if (e is MouseEventArgs mouseEventArgs && mouseEventArgs.Button == MouseButtons.Right)
            {
                HandleRightClick(icon);
            }
            else
            {
                System.Windows.MessageBox.Show("Balloon tip clicked!");
            }
        }

        private void HandleRightClick(NotifyIcon? icon)
        {
 
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _icon?.Dispose();
            base.OnExit(e);
        }
    }

}
