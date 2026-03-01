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

namespace DirectoryDash.ViewModels
{
    internal partial class ContainerViewModel : ObservableObject
    {
        private Func<ExplorerContainerData, ContainerViewModel> _containerVmFactory;
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

        public ContainerViewModel(
            ExplorerService explorerService,
            IconService iconService,
            ContainersStore containersStore,
            Func<ExplorerContainerData, ContainerViewModel> containerVmFactory,
            ExplorerContainerData data)
        {
            _containerVmFactory = containerVmFactory;
            _explorerService = explorerService;
            _iconService = iconService;
            ContainerData = data;
            ContainersStore = containersStore;

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
        }

        private void OpenInExplorer() => _explorerService.OpenFile(containerData.ElementPath);


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
