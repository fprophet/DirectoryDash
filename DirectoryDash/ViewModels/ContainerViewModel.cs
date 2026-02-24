using CommunityToolkit.Mvvm.ComponentModel;
using DirectoryDash.Models;
using DirectoryDash.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectoryDash.ViewModels
{
    internal partial class ContainerViewModel : ObservableObject
    {
        public ObservableCollection<ExplorerItem> Items { get; set; } = new ObservableCollection<ExplorerItem>();

        [ObservableProperty]
        private int xCoord;

        [ObservableProperty]
        private int yCoord;

        [ObservableProperty]
        private int width = 200;

        [ObservableProperty]
        private int height = 300;

        [ObservableProperty]
        private string elementName;

        [ObservableProperty]
        private int index = 0;


        public ContainerViewModel(ExplorerService explorerService, IconService iconService)
        {
            
        }
    }
}
