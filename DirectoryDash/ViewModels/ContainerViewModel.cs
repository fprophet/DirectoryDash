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


        public ICommand OnContainerClickCommand => new RelayCommand<ExplorerItem>(OnContainerClick);

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

            Initialize();
        }

        private void Initialize()
        {
            if (ContainerData == null) return;

            //create the nodes for the paths saved by user
            if (ContainerData.IsPathSelection)
            {
                var rootNodes = _explorerService.GetNodesFromSavedPaths(SettingsHelper.Settings.SavedPaths);

                foreach (var node in rootNodes)
                {
                    ContainerData.Items.Add(node);
                }
            }
            else
            {
                var rootNodes = _explorerService.GetNodes(ContainerData.ElementPath);

                foreach (var node in rootNodes)
                {
                    ContainerData.Items.Add(node);
                }
            }

            ItemListViewModel.UpdateCollection(ContainerData.Items);
        }

        [RelayCommand]
        private void OpenInExplorer(string path)
        {
            if (string.IsNullOrEmpty(path))
                _explorerService.OpenFile(containerData.ElementPath);
            else
                _explorerService.OpenFile(path);
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
        public void UnregisterContainer()
        {
            var found = ContainersStore.AllContainers.FirstOrDefault(x => x.ContainerData.ElementPath == ContainerData.ElementPath);
            if (found != null)
                ContainersStore.AllContainers.Remove(found);

            if( ChildContainer != null)
            {
                ChildContainer.UnregisterContainer();

                ChildContainer = null;
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
        private void SaveNavigationPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                path = ContainerData.ElementPath;

            var res = SettingsHelper.AddPath(path);
            
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
        private void StartRenameItem(ExplorerItem item) => item.IsEditing = true;

        [RelayCommand]
        private void SaveItemChanges(ExplorerItem item)
        {
            item.IsEditing = false;
            _explorerService.RenameItem(item.FullPath, item.Name);
            item.FullPath = System.IO.Path.Combine(ContainerData.ElementPath, item.Name);

            ItemListViewModel.Refresh();
        }

        [RelayCommand]
        private void OpenItemProperties(string path) => FileHelper.OpenFileProperties(path);

        [RelayCommand]
        private void CopyFileToClipboard(string path) => _explorerService.CopyFileToClipboard(path);

        [RelayCommand]
        private void CopyPathToClipboard(string path) => _explorerService.CopyPathToClipboard(path);

        private void UnregisterChildContainer()
        {
            if (ChildContainer == null) return;
            
            var found = ContainersStore.AllContainers.FirstOrDefault(x => x.ContainerData.ElementPath == ChildContainer.ContainerData.ElementPath);
            if (found != null)
                ContainersStore.AllContainers.Remove(found);
        
            ChildContainer.UnregisterContainer();
        
            ChildContainer = null;
        }

    }
}
