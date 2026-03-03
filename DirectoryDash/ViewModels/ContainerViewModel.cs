using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DirectoryDash.Factories;
using DirectoryDash.Helpers;
using DirectoryDash.Models;
using DirectoryDash.Services;
using DirectoryDash.Stores;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Shapes;

namespace DirectoryDash.ViewModels
{
    internal partial class ContainerViewModel : ObservableObject
    {
        private Func<ExplorerContainerData, ContainerViewModel> _containerVmFactory;
        private ExplorerService _explorerService;
        private IconService _iconService;

        public ContainersStore ContainersStore { get; }
        public ItemListViewModel ItemListViewModel { get; }

        [ObservableProperty]
        private ExplorerContainerData containerData = new ExplorerContainerData();

        [ObservableProperty]
        private ContainerViewModel childContainer;
        private bool _isLoaded = false;

        public ICommand OnContainerClickCommand => new RelayCommand<ExplorerItem>(OnContainerClick);
        public ICommand OnLoadedCommand => new AsyncRelayCommand(OnLoaded);

        public ContainerViewModel(
            ExplorerService explorerService,
            IconService iconService,
            ContainersStore containersStore,
            Func<ExplorerContainerData, ContainerViewModel> containerVmFactory,
            ItemListViewModel itemListViewModel,
            ExplorerContainerData data)
        {
            _containerVmFactory = containerVmFactory;
            _explorerService = explorerService;
            _iconService = iconService;
            ContainerData = data;
            ContainersStore = containersStore;
            ItemListViewModel = itemListViewModel;

        }

        private async Task InitializeAsync()
        {
            if (ContainerData == null) return;

            //create the nodes for the paths saved by user
            if (ContainerData.IsPathSelection)
            {
                var rootNodes = _explorerService.GetNodesFromSavedPaths(SettingsHelper.Settings.SavedPaths);
                ListHelper.UpdateCollection(ContainerData.Items, rootNodes);
            }
            else
            {
                var rootNodes = _explorerService.GetNodes(ContainerData.ElementPath);
                await ListHelper.AddInBatches(ContainerData.Items, rootNodes, fromDispatcher: true, batchSize: 10);
            }

            ItemListViewModel.UpdateCollection(ContainerData.Items);
        }

        private ContainerViewModel CreateContainerNode(string nodePath)
        {
            var data = ExplorerContainerDataFactory.CreateChildData(ContainerData, nodePath);
            ContainerViewModel containerViewModel = _containerVmFactory(data);
            ChildContainer = containerViewModel;
            return containerViewModel;
        }

        private void RegisterContainer(ContainerViewModel containerViewModel)
        {
            var found = ContainersStore.AllContainers.FirstOrDefault(x => x.ContainerData.ElementPath == containerViewModel.ContainerData.ElementPath);
            if (found == null)
                ContainersStore.AllContainers.Add(containerViewModel);
        }

        [RelayCommand]
        public async Task OnLoaded()
        {
            if (_isLoaded) return;
            _isLoaded = true;
            InitializeAsync();

            //await _explorerService.StartClear();
        }

        [RelayCommand]
        private void OpenInExplorer(string path)
        {
            if (string.IsNullOrEmpty(path))
                _explorerService.OpenFile(containerData.ElementPath);
            else
                _explorerService.OpenFile(path);
        }

        [RelayCommand]
        private void OnContainerClick(ExplorerItem item)
        {
            if (item.IsDirectory)
            {
                UnregisterChildContainer();
                var containerViewModel = CreateContainerNode(item.FullPath);
                RegisterContainer(containerViewModel);
            }
            else
            {
                _explorerService.OpenFile(item.FullPath);
            }
        }

        [RelayCommand]
        private void CreateFolder()
        {
           var path = _explorerService.CreateFolder(ContainerData.ElementPath);

            if (!string.IsNullOrEmpty(path))
            {
                var item = _explorerService.GetNode(path);
                ContainerData.Items.Add(item);
                ItemListViewModel.Refresh();
            }
        }

        [RelayCommand]
        private void CreateTextDoc()
        {
           var path = _explorerService.CreateTextDoc(ContainerData.ElementPath);

            if (!string.IsNullOrEmpty(path))
            {
                var item = _explorerService.GetNode(path);
                ContainerData.Items.Add(item);
                ItemListViewModel.Refresh();
            }
        }

        [RelayCommand]
        private void DeleteItem(string path)
        {
            var deleted = _explorerService.DeleteItem(path);

            if (deleted)
            {
                var item = ContainerData.Items.FirstOrDefault(x => x.FullPath == path);
                ContainerData.Items.Remove(item);
                ItemListViewModel.Refresh();
            }
        }

        [RelayCommand]
        private void SaveNavigationPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                path = ContainerData.ElementPath;

            var res = SettingsHelper.AddNavigationPath(path);
            
            if( !res ) return;

            //update the ui path selection container
            var pathSelectionContainer = ContainersStore.AllContainers.FirstOrDefault(x => x.ContainerData.IsPathSelection);
            if( pathSelectionContainer != null)
            {
                var item = _explorerService.GetNode(path);
                pathSelectionContainer.ContainerData.Items.Add(item);
                pathSelectionContainer.ItemListViewModel.Refresh();
            }
        }

        [RelayCommand]
        private void RemoveNavigationPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return;

            SettingsHelper.RemoveNavigationPath(path);

            //the container that is removing the paths is always the selection container
            if(ContainerData.IsPathSelection)
            {
                var item = ContainerData.Items.FirstOrDefault(x => x.FullPath == path);
                ContainerData.Items.Remove(item);

                //close all containers except the selection container
                if (ContainersStore.AllContainers.Count > 1
                        && ContainersStore.AllContainers[1].ContainerData.ElementPath == path)
                    ContainersStore.AllContainers[0].UnregisterChildContainer();
            }
        }

        [RelayCommand]
        private void StartRenameItem(ExplorerItem item) => item.IsEditing = true;

        [RelayCommand]
        private void SaveItemChanges(ExplorerItem item)
        {
            item.IsEditing = false;
            _explorerService.RenameItem(item.FullPath, item.Name);
            item.FullPath = System.IO.Path.Combine(ContainerData.ElementPath, item.Name);
        }

        [RelayCommand]
        private void OpenItemProperties(string path) => FileHelper.OpenFileProperties(path);

        [RelayCommand]
        private void CopyFileToClipboard(string path) => _explorerService.CopyFileToClipboard(path);

        [RelayCommand]
        private void CopyPathToClipboard(string path) => _explorerService.CopyPathToClipboard(path);

        [RelayCommand]
        public void UnregisterContainer()
        {
            var found = ContainersStore.AllContainers.FirstOrDefault(x => x.ContainerData.ElementPath == ContainerData.ElementPath);
            if (found != null)
                ContainersStore.AllContainers.Remove(found);

            if (ChildContainer != null)
            {
                ChildContainer.UnregisterContainer();

                ChildContainer = null;
            }
        }

        private void UnregisterChildContainer()
        {
            if (ChildContainer == null) return;
            
            var found = ContainersStore.AllContainers
                .FirstOrDefault(x => x.ContainerData.ElementPath == ChildContainer.ContainerData.ElementPath);

            if (found != null)
                ContainersStore.AllContainers.Remove(found);
        
            ChildContainer.UnregisterContainer();
        
            ChildContainer = null;
        }
    }
}
