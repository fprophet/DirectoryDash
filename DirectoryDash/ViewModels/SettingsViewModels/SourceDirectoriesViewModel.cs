using CommunityToolkit.Mvvm.Input;
using DirectoryDash.Factories;
using DirectoryDash.Helpers;
using DirectoryDash.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectoryDash.ViewModels.SettingsViewModels
{
    internal partial class SourceDirectoriesViewModel : BaseViewModel
    {
        private ItemFactory _itemFactory;
        private ExplorerService _explorerService;

        public ObservableCollection<string> SavedPaths { get; set; } = new ObservableCollection<string>();

        public SourceDirectoriesViewModel(ExplorerService explorerService, ItemFactory itemFactory)
        {
            _itemFactory = itemFactory;
            _explorerService = explorerService;
            foreach (var path in SettingsHelper.Settings.SavedPaths) { SavedPaths.Add(path); }
        }

        [RelayCommand]
        private void Browse()
        {
            var path = _explorerService.SelectDirectory();

            if (!string.IsNullOrEmpty(path))
                SavedPaths.Add(path);
        }

        [RelayCommand]
        private void RemovePath(string path)
        {
            SavedPaths.Remove(path);
        }
    }
}
