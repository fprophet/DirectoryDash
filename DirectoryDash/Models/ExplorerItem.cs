using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace DirectoryDash.Models
{
    internal partial class ExplorerItem : ObservableObject
    {
        [ObservableProperty]
        private string name;

        [ObservableProperty]
        public string fullPath;
        
        [ObservableProperty]
        public bool isDirectory;

        [ObservableProperty]
        public ImageSource icon;

        [ObservableProperty]
        public bool isEditing = false;
    }
}
