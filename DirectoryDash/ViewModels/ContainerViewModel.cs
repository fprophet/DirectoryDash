using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
    internal partial class ContainerViewModel : ObservableObject
    {
        [ObservableProperty]
        private ExplorerContainer containerData = new ExplorerContainer();
        private ExplorerService _explorerService;
        private IconService _iconService;

        public ICommand OpenInExplorerCommand => new RelayCommand(OpenInExplorer);

        public ContainerViewModel(ExplorerService explorerService, IconService iconService)
        {
            _explorerService = explorerService;
            _iconService = iconService;
        }

        private void OpenInExplorer()
        {
            _explorerService.OpenFile(containerData.ElementPath);
        }
    }
}
