using DirectoryDash.Helpers;
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

    internal partial class MainViewModel : BaseViewModel
    {
        public ObservableCollection<ExplorerNode> Nodes { get; set; } = new ObservableCollection<ExplorerNode>();

        private string title;
        private ExplorerService _explorerService;

        public MainViewModel(ExplorerService explorerService)
        {
            _explorerService = explorerService;

            var sourceDirectory = SettingsHelper.Settings.SourcePath;

            var rootNodes = _explorerService.GetNodes(sourceDirectory);

            foreach (var node in rootNodes)
            {
                Nodes.Add(node);
            }
        }
    }
}
