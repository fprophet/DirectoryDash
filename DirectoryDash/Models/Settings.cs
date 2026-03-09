using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DirectoryDash.Models
{
    internal partial class Settings : ObservableObject
    {
        [ObservableProperty]
        public List<string> savedPaths = new List<string>();

        [ObservableProperty]
        public bool onStartup = true;

        [ObservableProperty]
        public bool directoriesOnly = false;

        [ObservableProperty]
        public bool navigateOnHover = false;

        [ObservableProperty]
        public int clearViewDelay = 2000;

        [ObservableProperty]
        public bool clearViewOnLeave = true;

        [ObservableProperty]
        public bool toggleNavigation = false;

        [ObservableProperty]
        public HotKey navigationBlockHotKey = new HotKey() { Modifier = ModifierKeys.Control };

        [ObservableProperty]
        public HotKey clearViewHotKey = new HotKey() { Key = Key.Escape };
    }
}
