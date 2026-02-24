using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DirectoryDash.Helpers;
using DirectoryDash.Models;
using DirectoryDash.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DirectoryDash.ViewModels
{

    internal partial class MainViewModel : BaseViewModel
    {
        public ObservableCollection<ContainerViewModel> ContainerViewModels { get; set; } = new ObservableCollection<ContainerViewModel>();

        private string title;
        private IconService _iconService;
        private ExplorerService _explorerService;

        [ObservableProperty]
        private bool isListVisible = false;
        [ObservableProperty]
        private int parentListMaxHeight = 200;
        [ObservableProperty]
        private int parentListMaxWidth = 400;
        [ObservableProperty]
        private int currentIndex = 0;



        public ICommand OnContainerClickCommand => new RelayCommand<object>(OnContainerClick);
        public ICommand OnMouseLeaveCommand => new AsyncRelayCommand(OnMouseLeave);
        public ICommand OnMouseEnterCommand => new AsyncRelayCommand(OnMouseEnter);

        public MainViewModel(ExplorerService explorerService, IconService iconService)
        {
            _iconService = iconService;
            _explorerService = explorerService;

            SetSubscribers();
        }

        private void CreateRootContainer()
        {
            var sourceDirectory = SettingsHelper.Settings.SourcePath;
            var rootNodes = _explorerService.GetNodes(sourceDirectory);

            ContainerViewModel rootContainer = new ContainerViewModel(_explorerService, _iconService);

            foreach (var node in rootNodes)
            {
                rootContainer.Items.Add(node);
            }

            ContainerViewModels.Add(rootContainer);
        }

        [RelayCommand]
        private void OnContainerClick(object sender)
        {
            if (sender is not object[] parameters || parameters.Length != 2) return;

            if (parameters[0] is not ExplorerItem item || parameters[1] is not ContainerViewModel containerViewModel) return;

            if (item.IsDirectory)
            {
                var items = _explorerService.GetNodes(item.FullPath);
                CreateContainerNode(containerViewModel, items);
            }
            else
            {
                _explorerService.OpenFile(item.FullPath);
            }
        }

        private async Task OnMouseLeave() => await _explorerService.StartClear();

        private async Task OnMouseEnter() => await _explorerService.CancelClear();

        private void ClearContainers()
        {
            foreach (var container in ContainerViewModels)
            {
                container.Items.Clear();
            }
             ContainerViewModels.Clear();
        }

        private void CreateContainerNode(ContainerViewModel sender, List<ExplorerItem> items)
        {
            if( sender.Index < CurrentIndex)
                ClearFromIndex(sender.Index);

            var nextIndex = ContainerViewModels.Count;
            ClearContainersWithIndex(nextIndex);

            ContainerViewModel containerViewModel = new ContainerViewModel(_explorerService, _iconService);
            containerViewModel.XCoord = sender.XCoord - sender.Width - 20;
            containerViewModel.YCoord = sender.YCoord;
            foreach (var item in items)
            {
                containerViewModel.Items.Add(item);
            }
            CurrentIndex = containerViewModel.Index = nextIndex;
            ContainerViewModels.Add(containerViewModel);
        }

        private void ClearContainersWithIndex(int nextIndex)
        {
            var containers = ContainerViewModels.Where(x => x.Index > nextIndex).ToList();
            foreach (var container in containers)
            {
                ContainerViewModels.Remove(container);
            }
        }

        private void ClearFromIndex(int index)
        {
            var containers = ContainerViewModels.Where(x => x.Index > index).ToList();
            foreach (var container in containers)
            {
                ContainerViewModels.Remove(container);
            }
        }

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
            
            CreateRootContainer();
            var rootContainer = ContainerViewModels.FirstOrDefault();
            if (rootContainer == null) return;

            CurrentIndex = rootContainer.Index = 0;
            rootContainer.XCoord = _iconService.IconX - rootContainer.Width - 20;
            rootContainer.YCoord = _iconService.IconY - rootContainer.Height - 20;

            IsListVisible = true;
        }
    }
}
