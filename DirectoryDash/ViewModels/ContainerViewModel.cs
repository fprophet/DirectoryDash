using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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

namespace DirectoryDash.ViewModels
{
    internal partial class ContainerViewModel : ObservableObject
    {
        private ExplorerService _explorerService;
        private IconService _iconService;

        public ContainersStore ContainersStore { get; }

        [ObservableProperty]
        private ExplorerContainerData containerData = new ExplorerContainerData();

        [ObservableProperty]
        private ContainerViewModel childContainer;


        public ICommand OpenInExplorerCommand => new RelayCommand(OpenInExplorer);
        public ICommand OnContainerClickCommand => new RelayCommand<ExplorerItem>(OnContainerClick);
        public ICommand UnregisterContainerCommand => new RelayCommand(UnregisterContainer);

        public ContainerViewModel(ExplorerService explorerService, IconService iconService, ContainersStore containersStore)
        {
            _explorerService = explorerService;
            _iconService = iconService;
            ContainersStore = containersStore;
        }

        private void OpenInExplorer() => _explorerService.OpenFile(containerData.ElementPath);


        private ContainerViewModel CreateContainerNode(List<ExplorerItem> items, string nodePath)
        {

            ContainerViewModel containerViewModel = new ContainerViewModel(_explorerService, _iconService, ContainersStore);
            containerViewModel.ContainerData.ElementName = Path.GetFileName(nodePath);
            containerViewModel.ContainerData.ElementPath = nodePath;
            containerViewModel.ContainerData.XCoord = ContainerData.XCoord - ContainerData.Width - 20;
            containerViewModel.ContainerData.YCoord = ContainerData.YCoord;

            foreach (var item in items)
            {
                containerViewModel.ContainerData.Items.Add(item);
            }

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
                var items = _explorerService.GetNodes(item.FullPath);
                var containerViewModel = CreateContainerNode(items, item.FullPath);
                RegisterContainer(containerViewModel);

            }
            else
            {
                _explorerService.OpenFile(item.FullPath);
            }
        }

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
