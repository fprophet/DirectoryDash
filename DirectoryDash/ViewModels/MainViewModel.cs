using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DirectoryDash.Helpers;
using DirectoryDash.Models;
using DirectoryDash.Services;
using DirectoryDash.Stores;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DirectoryDash.ViewModels
{

    internal partial class MainViewModel : BaseViewModel
    {
        private DialogBoxService _dialogBoxService;
        private IconService _iconService;
        private ExplorerService _explorerService;
        private Func<ExplorerContainerData, ContainerViewModel> _containerVmFactory;
        private KeyService _keyService;
        private SettingsService _settingsService;

        public ContainersStore ContainersStore { get; }

        [ObservableProperty]
        private ContainerViewModel rootContainer;

        [ObservableProperty]
        private bool isListVisible = false;

        [ObservableProperty]
        private int currentIndex = 0;

        [ObservableProperty]
        private double listHolderWidth = Vars.ScreenWidth - 200; //100 margin l+r

        public HotKey ClearViewHotKey => SettingsHelper.Settings.ClearViewHotKey;
        public HotKey NavigationBlockHotKey => SettingsHelper.Settings.NavigationBlockHotKey;


        public ICommand OnMouseLeaveCommand => new AsyncRelayCommand(OnMouseLeave);
        public ICommand OnMouseEnterCommand => new AsyncRelayCommand(OnMouseEnter);

        public MainViewModel(
            ExplorerService explorerService,
            IconService iconService,
            ContainersStore containersStore,
            SettingsService settingsService,
            DialogBoxService dialogBoxService,
            KeyService keyService,
            Func<ExplorerContainerData, ContainerViewModel> containerVmFactory)
        {
            _dialogBoxService = dialogBoxService;
            _iconService = iconService;
            _explorerService = explorerService;
            _containerVmFactory = containerVmFactory;
            _keyService = keyService;
            _settingsService = settingsService;

            ContainersStore = containersStore;

            SetSubscribers();
        }

        private void CreateRootContainer()
        {
            var sourceDirectory = SettingsHelper.Settings.SavedPaths.First();
            RootContainer = _containerVmFactory(new ExplorerContainerData() 
                { 
                    ElementPath = sourceDirectory, 
                    ElementName = Path.GetFileName(sourceDirectory),
                });
        }

        private string AddNewPath() => _settingsService.SelectNewPath();

        private void CreateRootSelectionContainer()
        {
            var data = new ExplorerContainerData() { IsPathSelection = true };
            RootContainer = _containerVmFactory(data);
        }

        private async Task OnMouseLeave() => await _explorerService.StartClear();

        private async Task OnMouseEnter() => await _explorerService.CancelClear();

        private void ClearContainers() => RootContainer?.UnregisterContainer();

        private void SetSubscribers()
        {
            _iconService.IconClick += IconService_HandleClick;
            _explorerService.Clear += ClearView;
        }

        public void BlockNavigationStart()
        {
            ContainersStore.NavigationBlocked = true;
        }

        public void BlockNavigationEnd()
        {
            ContainersStore.NavigationBlocked = false;
        }

        private void IconService_HandleClick(object? sender, EventArgs e)
        {
            ClearView();

            if (SettingsHelper.Settings.SavedPaths.Count == 0)
            {
                _dialogBoxService.InfoBox("No saved paths found! Please add a path before starting navigation.");
                AddNewPath();
                return;
            }

            //if (SettingsHelper.Settings.SavedPaths.Count > 1)
                CreateRootSelectionContainer();
            //else
            //    CreateRootContainer();

            if (RootContainer == null) return;

            CurrentIndex = RootContainer.ContainerData.Index = 0;
            ContainersStore.AllContainers.Add(RootContainer);

            IsListVisible = true;
            BlockNavigationEnd();
            Vars.Reset();
        }

        [RelayCommand]
        private void ClearView()
        {
            ClearContainers();
            IsListVisible = false;
        }

        [RelayCommand]
        private void KeyDown(InputEventArgs e)
        {
            if (e == null) return;

            if (e is not System.Windows.Input.KeyEventArgs args) return;

            if (_keyService.MatchGesture(args, NavigationBlockHotKey) && !SettingsHelper.Settings.ToggleNavigation)
                BlockNavigationStart();

            if (_keyService.MatchGesture(args, ClearViewHotKey))
                ClearView();
        }

        [RelayCommand]
        private void KeyUp(InputEventArgs e)
        {
            if (e is not System.Windows.Input.KeyEventArgs args) return;

            if (_keyService.MatchGesture(args, NavigationBlockHotKey) && SettingsHelper.Settings.ToggleNavigation)
                ToggleNavigation();
            else if(_keyService.MatchGesture(args, NavigationBlockHotKey) && !SettingsHelper.Settings.ToggleNavigation)
                BlockNavigationEnd();
        }

        private void ToggleNavigation()
        {
            ContainersStore.NavigationBlocked = !ContainersStore.NavigationBlocked;
        }
    }
}
