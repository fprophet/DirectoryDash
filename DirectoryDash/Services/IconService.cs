using DirectoryDash.Factories;
using DirectoryDash.ViewModels;
using DirectoryDash.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms; // Add this using directive at the top

namespace DirectoryDash.Services
{
    internal class IconService
    {
        public event EventHandler IconClick;

        private ItemFactory _itemFactory;
        private NotifyIcon _icon;

        public int IconX { get; private set; }
        public int IconY { get; private set; }

        public IconService(ItemFactory itemFactory)
        {
            _itemFactory = itemFactory;

            _icon = new NotifyIcon();
            _icon.Icon = new Icon("tray.ico");
            _icon.Visible = true;
            _icon.Click += HandleClick;
            _icon.ContextMenuStrip = new ContextMenuStrip();
            _icon.ContextMenuStrip.Items.Add("Settings", null, OpenSettingsWindow);
            _icon.ContextMenuStrip.Items.Add("-", null);
            _icon.ContextMenuStrip.Items.Add("Exit", null, (s, args) => System.Windows.Application.Current.Shutdown());
            _icon.ContextMenuStrip.Show();
        }

        private void OpenSettingsWindow(object? sender, EventArgs e)
        {

            var vm = _itemFactory.Create<SettingsViewModel>();
            var settingsWindow = new SettingsWindow();
            settingsWindow.DataContext = vm;
            settingsWindow.Show();

        }

        public void OnIconClick()
        {
            IconClick?.Invoke(this, EventArgs.Empty);
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
                MainWindow mainWindow = (MainWindow)System.Windows.Application.Current.MainWindow;
                if (mainWindow != null)
                {
                    mainWindow.Show();
                    mainWindow.Activate();
                    mainWindow.Focus();
                }


                var (x, y) = GetCurrentMousePosition();
                IconX = x;
                IconY = y;

                OnIconClick();
            }
        }

        private (int x, int y) GetCurrentMousePosition()
        {
            var clickPoint = Control.MousePosition;
            return (clickPoint.X, clickPoint.Y);
        }

        private void HandleRightClick(NotifyIcon? icon)
        {

        }
    }
}
