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
        private int canvaX;
        [ObservableProperty]
        private int canvaY;
        [ObservableProperty]
        private bool isListVisible = false;
        [ObservableProperty]
        private int parentListMaxHeight = 200;
        [ObservableProperty]
        private int parentListMaxWidth = 400;

        public ICommand OnContainerClickCommand => new RelayCommand<object>(OnContainerClick);
        public MainViewModel(ExplorerService explorerService, IconService iconService)
        {
            _iconService = iconService;
            _explorerService = explorerService;

            SetSubscribers();

            var sourceDirectory = SettingsHelper.Settings.SourcePath;
            var rootNodes = _explorerService.GetNodes(sourceDirectory);

            ContainerViewModel rootContainer = new ContainerViewModel(explorerService, iconService);
            rootContainer.ElementName = "CONTAINER_1";

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

            var items = _explorerService.GetNodes(item.FullPath);
            CreateContainerNode(containerViewModel, items);
        }

        private void CreateContainerNode(ContainerViewModel sender, List<ExplorerItem> items)
        {
            ContainerViewModel containerViewModel = new ContainerViewModel(_explorerService, _iconService);
            containerViewModel.XCoord = sender.XCoord - sender.Width;
            containerViewModel.YCoord = sender.YCoord - (sender.Height / 2);
            containerViewModel.ElementName = "CONTAINER_" + ContainerViewModels.Count;
            foreach (var item in items)
            {
                containerViewModel.Items.Add(item);
            }
            ContainerViewModels.Add(containerViewModel);
        }

        private void SetSubscribers()
        {
            _iconService.IconClick += (s, e) =>
            {
                //CanvaX = _iconService.IconX - ParentListMaxWidth - 20;
                //CanvaY = _iconService.IconY - ParentListMaxHeight - 20;

                var rootContainer = ContainerViewModels.FirstOrDefault();
                if (rootContainer == null) return;

                rootContainer.XCoord = _iconService.IconX - rootContainer.Width - 20;
                rootContainer.YCoord = _iconService.IconY - rootContainer.Height - 20;

                IsListVisible = true;
            };
        }
    }
}
