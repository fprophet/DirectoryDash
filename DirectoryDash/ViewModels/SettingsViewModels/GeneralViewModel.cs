using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DirectoryDash.Helpers;
using DirectoryDash.Models;
using DirectoryDash.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DirectoryDash.ViewModels.SettingsViewModels
{
    internal partial class GeneralViewModel : BaseViewModel
    {
        [ObservableProperty]
        private bool onStartup;

        [ObservableProperty]
        private bool directoriesOnly;

        [ObservableProperty]
        private bool navigateOnHover;

        [ObservableProperty]
        private bool clearViewOnLeave;

        [ObservableProperty]
        private int clearViewDelay;

        [ObservableProperty]
        private HotKey navigationBlockHotKey;

        [ObservableProperty]
        private HotKey clearViewHotKey;

        [ObservableProperty]
        private string navigationBlockHotKeyString;

        [ObservableProperty]
        private string clearViewHotKeyString;

        [ObservableProperty]
        private bool toggleNavigation;

        [ObservableProperty]
        private bool isModalVisible = false;
        
        private bool _isChangingClearViewHk = false;
        private bool _isChangingNavigationHk = false;

        private KeyService _keyService;

        public GeneralViewModel(KeyService keyService) 
        {
            _keyService = keyService;

            OnStartup = SettingsHelper.Settings.OnStartup;
            DirectoriesOnly = SettingsHelper.Settings.DirectoriesOnly;
            NavigateOnHover = SettingsHelper.Settings.NavigateOnHover;
            ClearViewDelay = SettingsHelper.Settings.ClearViewDelay;
            ClearViewOnLeave = SettingsHelper.Settings.ClearViewOnLeave;
            NavigationBlockHotKey = SettingsHelper.Settings.NavigationBlockHotKey;
            ClearViewHotKey = SettingsHelper.Settings.ClearViewHotKey;
            NavigationBlockHotKeyString = SettingsHelper.Settings.NavigationBlockHotKey.GetString();
            ClearViewHotKeyString = SettingsHelper.Settings.ClearViewHotKey.GetString();
            ToggleNavigation = SettingsHelper.Settings.ToggleNavigation;
        }

        partial void OnClearViewDelayChanged(int value)
        {
            if (value > 90000)
                ClearViewDelay = 10000;

            if (value < 0)
                ClearViewDelay = 0;
        }

        [RelayCommand]
        private void ChangeClearViewHk()
        {
            IsModalVisible = true;
            _isChangingClearViewHk = true;
        }

        [RelayCommand]
        private void ChangeNavigationBlockHk()
        {
            IsModalVisible = true;
            _isChangingNavigationHk = true;
        }

        [RelayCommand]
        private void KeyUp(InputEventArgs e)
        {
            if (IsModalVisible)
                ListenToGesture(e);
        }

        private void ListenToGesture(InputEventArgs e)
        {
            HotKey input = _keyService.DetectGesture(e);
            if (input == null)
            {
                IsModalVisible = false;
                System.Windows.MessageBox.Show("Invalid key combination.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if(_isChangingClearViewHk)
            {
                ClearViewHotKey = input;
                ClearViewHotKeyString = input.GetString();
            }

            if (_isChangingNavigationHk)
            {
                NavigationBlockHotKey = input;
                NavigationBlockHotKeyString = input.GetString();
            }

            _isChangingClearViewHk = false;
            _isChangingNavigationHk = false;
            IsModalVisible = false;
        }
    }
}
