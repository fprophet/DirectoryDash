using CommunityToolkit.Mvvm.ComponentModel;
using DirectoryDash.Helpers;
using DirectoryDash.Models;
using DirectoryDash.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectoryDash.Stores
{
    internal partial class ContainersStore : ObservableObject
    {
        public ObservableCollection<ContainerViewModel> AllContainers { get; } = new ObservableCollection<ContainerViewModel>();

        public ObservableCollection<ContainerViewModel> Workspace { get; } = new ObservableCollection<ContainerViewModel>();

        [ObservableProperty]
        private bool navigationBlocked = false;

        public bool CanFitNewContainer()
        {
            var lastContainer = AllContainers.Last();
            if (lastContainer == null) return true;

            var xCoord = lastContainer.ContainerData.XCoord - Vars.ContainerWidth - Vars.ContainerHorMargin;

            if (xCoord < 0)
                return false;

            return true;
        }

        internal void AddPrevToWorkspace()
        {
            ListHelper.AddToCollection(Workspace, AllContainers);
            AllContainers.Clear();
        }
    }
}
