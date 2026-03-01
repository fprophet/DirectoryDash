using CommunityToolkit.Mvvm.ComponentModel;
using DirectoryDash.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectoryDash.Stores
{
    internal class ContainersStore : ObservableObject
    {
        public ObservableCollection<ContainerViewModel> AllContainers { get; } = new ObservableCollection<ContainerViewModel>();
    }
}
