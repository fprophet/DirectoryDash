using CommunityToolkit.Mvvm.ComponentModel;
using DirectoryDash.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectoryDash.Models
{
    internal partial class ExplorerContainerData : ObservableObject
    {
        public ObservableCollection<ExplorerItem> Items { get; set; } = new ObservableCollection<ExplorerItem>();

        [ObservableProperty]
        private double xCoord = Vars.StartXCoord;

        [ObservableProperty]
        private double yCoord = Vars.StartYCoord;

        [ObservableProperty]
        private bool isVisible = true;

        [ObservableProperty]
        private bool isInWorkspace = false;

        [ObservableProperty]
        private double width = Vars.ContainerWidth;

        [ObservableProperty]
        private double height = Vars.ContainerHeight;

        [ObservableProperty]
        private string elementName;

        [ObservableProperty]
        private string elementPath;

        [ObservableProperty]
        private int index = 0;

        [ObservableProperty]
        private bool isPathSelection = false;
    }
}
