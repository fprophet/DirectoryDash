using CommunityToolkit.Mvvm.Input;
using DirectoryDash.Helpers;
using DirectoryDash.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectoryDash.ViewModels
{
    internal partial class SettingsViewModel : BaseViewModel
    {
        private ExplorerService _explorerService;

        public ObservableCollection<string> SavedPaths { get; set; } = new ObservableCollection<string>();

        public SettingsViewModel(ExplorerService explorerService)
        {
            _explorerService = explorerService;
            foreach (var path in SettingsHelper.Settings.SavedPaths) { SavedPaths.Add(path); }
        }

        [RelayCommand]
        private void Browse()
        {
            var path = _explorerService.SelectDirectory();

            if(!string.IsNullOrEmpty(path))
                SavedPaths.Add(path);
        }
  
        [RelayCommand]
        private void RemovePath(string path)
        {
            SavedPaths.Remove(path);
        }

        [RelayCommand]
        private void Save()
        {
            SettingsHelper.Settings.SavedPaths.Clear();

            foreach (var path in SavedPaths)
                SettingsHelper.Settings.SavedPaths.Add(path);

            SettingsHelper.SaveSettings();
        }

    }
}
