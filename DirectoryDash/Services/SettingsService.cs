using DirectoryDash.Enums;
using DirectoryDash.Factories;
using DirectoryDash.Helpers;
using DirectoryDash.SettingsViewModels.ViewModels;
using DirectoryDash.Views.SettingsViews;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectoryDash.Services
{
    internal class SettingsService
    {
        private bool _isSettingsOpen = false;
        private ItemFactory _itemFactory;
        private ExplorerService _explorerService;
        private SettingsWindow _settingsWindow;

        public SettingsService(ItemFactory itemFactory, ExplorerService explorerService)
        {
            _itemFactory = itemFactory;
            _explorerService = explorerService;
        }

        public void OpenSettingsWindow(SettingsSection section = SettingsSection.General)
        {
            if( _isSettingsOpen ) return;

            var vm = _itemFactory.Create<SettingsViewModel>();
            _settingsWindow = new SettingsWindow();
            _settingsWindow.DataContext = vm;
            vm.ChangeSection(section);
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

        internal string SelectNewPath()
        {
            var path = _explorerService.SelectDirectory();

            if( string.IsNullOrEmpty(path) 
                || !Directory.Exists(path)) return string.Empty;

            SettingsHelper.AddNavigationPath(path);

            return path;
        }
    }
}
