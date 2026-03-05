using CommunityToolkit.Mvvm.ComponentModel;
using DirectoryDash.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectoryDash.ViewModels.SettingsViewModels
{
    internal partial class GeneralViewModel : BaseViewModel
    {
        [ObservableProperty]
        private bool onStartup;

        [ObservableProperty]
        private bool directoriesOnly;

        [ObservableProperty]
        private bool navigateOnHover;

        public GeneralViewModel() 
        {
            OnStartup = SettingsHelper.Settings.OnStartup;
            DirectoriesOnly = SettingsHelper.Settings.DirectoriesOnly;
            NavigateOnHover = SettingsHelper.Settings.NavigateOnHover;
        }
    }
}
