using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectoryDash.Models
{
    internal partial class ExplorerContainer : ObservableObject
    {
        public ObservableCollection<ExplorerItem> Items { get; set; } = new ObservableCollection<ExplorerItem>();

        [ObservableProperty]
        private int xCoord;

        [ObservableProperty]
        private int yCoord;
 
        [ObservableProperty]
        private int width;

        [ObservableProperty]
        private int height;
    }
}
