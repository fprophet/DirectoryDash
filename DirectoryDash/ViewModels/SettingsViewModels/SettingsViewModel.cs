using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DirectoryDash.Enums;
using DirectoryDash.Factories;
using DirectoryDash.Helpers;
using DirectoryDash.Services;
using DirectoryDash.ViewModels;
using DirectoryDash.ViewModels.SettingsViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DirectoryDash.SettingsViewModels.ViewModels
{
    internal partial class SettingsViewModel : BaseViewModel
    {
        private SettingsService _settingsService;
        private ItemFactory _itemFactory;
        private ExplorerService _explorerService;
        private DialogBoxService _dialogBoxService;

        public GeneralViewModel GeneralViewModel { get; }
        public SourceDirectoriesViewModel SourceDirectoriesViewModel { get; }
        public InfoViewModel InfoViewModel { get; }

        [ObservableProperty]
        private BaseViewModel currentViewModel;

        [ObservableProperty]
        private SettingsSection currentSection = SettingsSection.General;

        public SettingsViewModel(
            ExplorerService explorerService,
            SettingsService settingsService,
            GeneralViewModel generalViewModel,
            DialogBoxService dialogBoxService,
            SourceDirectoriesViewModel sourceDirectoriesViewModel,
            InfoViewModel infoViewModel,
            ItemFactory itemFactory)
        {
            _settingsService = settingsService;
            _itemFactory = itemFactory;
            _explorerService = explorerService;
            _dialogBoxService = dialogBoxService;

            GeneralViewModel = generalViewModel;
            SourceDirectoriesViewModel = sourceDirectoriesViewModel;
            InfoViewModel = infoViewModel;

            CurrentViewModel = GeneralViewModel;
        }


        [RelayCommand]
        public void ChangeSection(SettingsSection section)
        {
            switch (section)
            {
                case SettingsSection.General:
                    CurrentViewModel = GeneralViewModel;
                    CurrentSection = SettingsSection.General;
                    break;
                case SettingsSection.SourceDirectories:
                    CurrentViewModel = SourceDirectoriesViewModel;
                    CurrentSection = SettingsSection.SourceDirectories;
                    break;
                case SettingsSection.Info:
                    CurrentViewModel = InfoViewModel;
                    CurrentSection = SettingsSection.Info;
                    break;
            }
        }

        [RelayCommand]
        private void Save()
        {
            SettingsHelper.Settings.SavedPaths.Clear();

            foreach (var path in SourceDirectoriesViewModel.SavedPaths)
                SettingsHelper.Settings.SavedPaths.Add(path);

            SettingsHelper.Settings.OnStartup = GeneralViewModel.OnStartup;
            SettingsHelper.Settings.DirectoriesOnly = GeneralViewModel.DirectoriesOnly;
            SettingsHelper.Settings.NavigateOnHover = GeneralViewModel.NavigateOnHover;
            SettingsHelper.Settings.ClearViewDelay = GeneralViewModel.ClearViewDelay;
            SettingsHelper.Settings.ClearViewOnLeave = GeneralViewModel.ClearViewOnLeave;
            SettingsHelper.Settings.ClearViewHotKey = GeneralViewModel.ClearViewHotKey;
            SettingsHelper.Settings.NavigationBlockHotKey = GeneralViewModel.NavigationBlockHotKey;
            SettingsHelper.Settings.ToggleNavigation = GeneralViewModel.ToggleNavigation;

            SettingsHelper.SaveSettings();

            _dialogBoxService.InfoBox("Settings saved.");
        }

        [RelayCommand]
        private void Cancel() => _settingsService.Close();
    }
}
