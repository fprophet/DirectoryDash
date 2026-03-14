using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DirectoryDash.Factories;
using DirectoryDash.Helpers;
using DirectoryDash.Models;
using DirectoryDash.Services;
using DirectoryDash.Stores;
using System.Windows.Input;
using System.Windows.Shapes;

namespace DirectoryDash.ViewModels
{
    internal partial class ContainerViewModel : ObservableObject
    {
        private Func<ExplorerContainerData, ContainerViewModel> _containerVmFactory;
        private ExplorerService _explorerService;
        private SettingsService _settingsService;

        public ContainersStore ContainersStore { get; }
        public ItemListViewModel ItemListViewModel { get; }

        [ObservableProperty]
        private ExplorerContainerData containerData = new ExplorerContainerData();

        [ObservableProperty]
        private ContainerViewModel childContainer;

        [ObservableProperty]
        private bool isLoading = false;

        //this is needed to prevent mouse leave from triggering
        //when oppening a context menu
        [ObservableProperty]
        private bool isItemContextMenuOpened = false;
        [ObservableProperty]
        private bool isContainerContextMenuOpened = false;

        public ICommand OnContainerClickCommand => new RelayCommand<ExplorerItem>(OnContainerClick);
        public ICommand OnMouseEnterItemCommand => new RelayCommand<ExplorerItem>(OnMouseEnterItem);
        public ICommand OnLoadedCommand => new AsyncRelayCommand(OnLoaded);

        public ContainerViewModel(
            ExplorerService explorerService,
            SettingsService settingsService,
            ContainersStore containersStore,
            Func<ExplorerContainerData, ContainerViewModel> containerVmFactory,
            ItemListViewModel itemListViewModel,
            ExplorerContainerData data)
        {
            _containerVmFactory = containerVmFactory;
            _explorerService = explorerService;
            _settingsService = settingsService;
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
            IsLoading = false;
        }

        private ContainerViewModel CreateContainerNode(string nodePath, bool isPositionedAtStart = false)
        {
            var data = ExplorerContainerDataFactory.CreateChildData(ContainerData, nodePath, isPositionedAtStart);
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
            if (IsLoading) return;
            IsLoading = true;
            await InitializeAsync();
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
        private void OnContainerClick(ExplorerItem item) => NavigateToNode(item);

        [RelayCommand]
        private void OnMouseEnterItem(ExplorerItem item)
        {
            if (!SettingsHelper.Settings.NavigateOnHover) return;

            NavigateToNode(item, false);
        }

        [RelayCommand]
        private void CreateFolder()
        {
            var path = _explorerService.CreateFolder(ContainerData.ElementPath);
            AddItem(path);
        }

        [RelayCommand]
        private void CreateTextDoc()
        {
           var path = _explorerService.CreateTextDoc(ContainerData.ElementPath);
            AddItem(path);
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
            ContainerData.IsVisible = false;

            var found = ContainersStore.AllContainers.FirstOrDefault(x => x.ContainerData.ElementPath == ContainerData.ElementPath);
            if (found != null)
                ContainersStore.AllContainers.Remove(found);
            ContainerData.Items.Clear();

            if (ChildContainer != null)
            {
                ChildContainer.UnregisterContainer();
                ChildContainer = null;
            }

        }

        [RelayCommand]
        public void AddNewPath()
        {
            var path = _settingsService.SelectNewPath();

            if (string.IsNullOrEmpty(path)) return;

            if( ContainerData.IsPathSelection )
            {
                var item = _explorerService.GetNode(path);
                ContainerData.Items.Add(item);
                ItemListViewModel.Refresh();
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

        private void NavigateToNode(ExplorerItem item, bool isClick = true)
        {
            if (item == null) return;

            if( ContainersStore.NavigationBlocked ) return;

            if (item.IsDirectory)
            {
                UnregisterChildContainer();

                var vm = CreateContainerNode(item.FullPath);
                RegisterContainer(vm);
            }
            else if (isClick)
            {
                _explorerService.OpenFile(item.FullPath);
            }
        }

        private void AddItem(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                var item = _explorerService.GetNode(path);
                ContainerData.Items.Add(item);
                ItemListViewModel.Refresh();
            }
        }
    }
}
