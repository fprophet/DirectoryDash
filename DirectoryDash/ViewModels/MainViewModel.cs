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
        [ObservableProperty]
        private ContainerViewModel rootContainer;

        private IconService _iconService;
        private ExplorerService _explorerService;
        private Func<ExplorerContainerData, ContainerViewModel> _containerVmFactory;

        public ContainersStore ContainersStore { get; }

        [ObservableProperty]
        private bool isListVisible = false;

        [ObservableProperty]
        private int currentIndex = 0;

        [ObservableProperty]
        private double listHolderWidth = Vars.ScreenWidth - 200; //100 margin l+r

        public ICommand OnMouseLeaveCommand => new AsyncRelayCommand(OnMouseLeave);
        public ICommand OnMouseEnterCommand => new AsyncRelayCommand(OnMouseEnter);

        public MainViewModel(
            ExplorerService explorerService,
            IconService iconService,
            ContainersStore containersStore,
            Func<ExplorerContainerData, ContainerViewModel> containerVmFactory)
        {
            _iconService = iconService;
            _explorerService = explorerService;
            _containerVmFactory = containerVmFactory;
            ContainersStore = containersStore;

            SetSubscribers();
        }

        private void CreateRootContainer()
        {
            var sourceDirectory = SettingsHelper.Settings.SavedPaths.First();
            RootContainer = _containerVmFactory(new ExplorerContainerData() { ElementPath = sourceDirectory });
        }

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

        private void ClearView()
        {
            ClearContainers();
            IsListVisible = false;
        }

        private void IconService_HandleClick(object? sender, EventArgs e)
        {
            ClearView();
         
            if( SettingsHelper.Settings.SavedPaths.Count > 1)
                CreateRootSelectionContainer();
            else
                CreateRootContainer();

            if (RootContainer == null) return;

            CurrentIndex = RootContainer.ContainerData.Index = 0;
            ContainersStore.AllContainers.Add(RootContainer);

            IsListVisible = true;

            Vars.Reset();
        }

    }
}
