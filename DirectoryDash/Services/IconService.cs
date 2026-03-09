using DirectoryDash.Enums;
using DirectoryDash.Factories;
using DirectoryDash.Helpers;
using DirectoryDash.SettingsViewModels.ViewModels;
using DirectoryDash.ViewModels;
using DirectoryDash.Views;
using DirectoryDash.Views.SettingsViews;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms; // Add this using directive at the top

namespace DirectoryDash.Services
{
    internal class IconService
    {
        public event EventHandler IconClick;

        private SettingsService _settingsService;
        private ItemFactory _itemFactory;
        private NotifyIcon _icon;

        public double IconX { get; private set; }
        public double IconY { get; private set; }

        public IconService(ItemFactory itemFactory, SettingsService settingsService)
        {
            _settingsService = settingsService;
            _itemFactory = itemFactory;

            _icon = new NotifyIcon();
            _icon.Icon = new Icon("Resources/tray.ico");
            _icon.Visible = true;
            _icon.Click += HandleClick;
            _icon.ContextMenuStrip = new ContextMenuStrip();
            _icon.ContextMenuStrip.Items.Add("About", GetImage("info.png"), OpenAbout);
            _icon.ContextMenuStrip.Items.Add("New Path", GetImage("folder-copy.png"), AddPathAndOpen);
            _icon.ContextMenuStrip.Items.Add("Settings", GetImage("settings.png"), OpenSettingsWindow);
            _icon.ContextMenuStrip.Items.Add("-", null);
            _icon.ContextMenuStrip.Items.Add("Exit", GetImage("exit.png"), CloseApp);
            _icon.ContextMenuStrip.Show();
        }

        private void CloseApp(object? sender, EventArgs e) => System.Windows.Application.Current.Shutdown();

        private Image? GetImage(string v)
        {
            try
            {
                return Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources/Images/", v));
            }
            catch
            {
                return null;
            }
        }

        private void OpenSettingsWindow(object? sender, EventArgs e) => _settingsService.OpenSettingsWindow();

        private void OpenAbout(object? sender, EventArgs e) => _settingsService.OpenSettingsWindow(SettingsSection.Info);

        private void AddPathAndOpen(object? sender, EventArgs e)
        {
            _settingsService.SelectNewPath();
            OnIconClick();
        }

        public void OnIconClick()
        {
            IconClick?.Invoke(this, EventArgs.Empty);
        }

        private void HandleClick(object? sender, EventArgs e)
        {
            var icon = sender as NotifyIcon;
            if (e is MouseEventArgs mouseEventArgs && mouseEventArgs.Button == MouseButtons.Left)
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

                Vars.GetStartCoordinates(x,y);

                Vars.MouseYPosition = y;
                //IconX = width;
                //IconY = height;

                OnIconClick();
            }
        }

        private (int x, int y) GetCurrentMousePosition()
        {
            var clickPoint = Control.MousePosition;
            return (clickPoint.X, clickPoint.Y);
        }

    }
}
