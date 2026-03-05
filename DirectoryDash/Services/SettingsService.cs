using DirectoryDash.Factories;
using DirectoryDash.SettingsViewModels.ViewModels;
using DirectoryDash.Views.SettingsViews;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectoryDash.Services
{
    internal class SettingsService
    {
        private bool _isSettingsOpen = false;
        private ItemFactory _itemFactory;
        private SettingsWindow _settingsWindow;

        public SettingsService(ItemFactory itemFactory)
        {
            _itemFactory = itemFactory;
        }

        public void OpenSettingsWindow(object? sender, EventArgs e)
        {
            if( _isSettingsOpen ) return;

            var vm = _itemFactory.Create<SettingsViewModel>();
            _settingsWindow = new SettingsWindow();
            _settingsWindow.DataContext = vm;
            _settingsWindow.Show();
            _isSettingsOpen = true;

            _settingsWindow.Closing += SettingsWindow_Closing;
        }

        private void SettingsWindow_Closing(object? sender, CancelEventArgs e)
        {
            _isSettingsOpen = false;
            _settingsWindow.Closing -= SettingsWindow_Closing;
        }

        internal void Close()
        {
            _settingsWindow.Close();
            _isSettingsOpen = false;
        }
    }
}
