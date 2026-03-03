using DirectoryDash.Helpers;
using DirectoryDash.Models;
using DirectoryDash.Stores;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DirectoryDash.Services
{
    internal class ExplorerService
    {
        public Action Clear { get; internal set; }

        private Dictionary<string, ImageSource> _iconCache = new Dictionary<string, ImageSource>();

        private CancellationTokenSource _clearViewCT = new CancellationTokenSource();
        private ContainersStore _containersStore;

        public ExplorerService(ContainersStore containersStore)
        {
            _containersStore = containersStore;
        }

        public List<ExplorerItem> GetNodes(string path)
        {
            try
            {
                if (!Directory.Exists(path))
                {
                    Trace.WriteLine($"Directory does not exist: {path}");
                }

                var entires = Directory.GetFileSystemEntries(path);

                List<ExplorerItem> nodes = new List<ExplorerItem>();
                foreach (var entry in entires)
                {
                    var icon = FileIconHelper.GetSmallIcon(entry);
                    var node = new ExplorerItem()
                    {
                        Name = Path.GetFileName(entry),
                        FullPath = entry,
                        //Icon = IconToImageSource(icon),
                        IsDirectory = Directory.Exists(entry)
                    };
                    nodes.Add(node);
                }

                Task.Run(() => GetIconsForNodes(nodes));

                return nodes;
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log them)
                Trace.WriteLine($"Error accessing path: {ex.Message}");
                return new List<ExplorerItem>();
            }
        }

        public void GetIconsForNodes(List<ExplorerItem> nodes)
        {
            foreach (var node in nodes)
            {
                var icon = FileIconHelper.GetSmallIcon(node.FullPath);
                var imageSource = IconToImageSource(icon);
                imageSource.Freeze();
                string key;

                //if (node.IsDirectory)
                //    key = "folder";
                //else if (Path.GetExtension(node.FullPath) == ".exe")
                //    key = node.FullPath; 
                //else
                //    key = Path.GetExtension(node.FullPath).ToLower();

                //_iconCache[key] = imageSource;
                node.Icon = imageSource;
            }

            //foreach (var node in nodes)
            //{
            //    if (node.IsDirectory)
            //        node.Icon = _iconCache["folder"];
            //    else if (Path.GetExtension(node.FullPath) == ".exe")
            //        node.Icon = _iconCache[node.FullPath];
            //    else
            //        node.Icon = _iconCache[Path.GetExtension(node.FullPath).ToLower()];
            //}
        }

        internal void OpenFile(string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath)
                || (!File.Exists(fullPath)
                    && !Directory.Exists(Path.GetDirectoryName(fullPath)))) 
            {
                return;
            }
            Process.Start(new ProcessStartInfo  
            {
                FileName = fullPath,
                UseShellExecute = true
            });
        }

        internal async Task StartClear()
        {
            try
            {
                _clearViewCT = new CancellationTokenSource();
                await Task.Delay(2000, _clearViewCT.Token);
                //if (!_clearViewCT.IsCancellationRequested)
                //    Clear?.Invoke();
            }
            catch (TaskCanceledException) { }
        }

        internal async Task CancelClear() => _clearViewCT.Cancel();

        public ImageSource IconToImageSource(Icon icon)
        {
            return Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }

        internal string SelectDirectory()
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.ShowDialog();
            return dialog.SelectedPath;
        }

        internal IEnumerable<ExplorerItem> GetNodesFromSavedPaths(List<string> savedPaths)
        {
            var items = new List<ExplorerItem>();
            foreach (var node in SettingsHelper.Settings.SavedPaths)
            {
                items.Add(new ExplorerItem
                {
                    FullPath = node,
                    Name = Path.GetFileName(node),
                    IsDirectory = true,
                });
            }

            GetIconsForNodes(items);

            return items;
        }

        internal void CopyPathToClipboard(string path) => System.Windows.Clipboard.SetText(path);

        internal void CopyFileToClipboard(string path)
        {
            if (!File.Exists(path))
                return;

            StringCollection pathList = new StringCollection();
            pathList.Add(path);
            System.Windows.Clipboard.Clear();
            System.Windows.Clipboard.SetFileDropList(pathList);
        }

        internal void OpenItemProperties(string path)
        {
            if (!File.Exists(path) && !Directory.Exists(path))
                return;

            Process.Start(new ProcessStartInfo
            {
                FileName = path,
                UseShellExecute = true,
                Verb = "properties"
            });
        }


        //implement error handling and error messages
        internal bool DeleteItem(string path)
        {
            if( !File.Exists(path) && !Directory.Exists(path))
                return false;

            if( File.Exists(path))
                return DeleteFile(path);
            else
                return DeleteDirectory(path);
        }

        private bool DeleteDirectory(string path)
        {
            var confirmation = System.Windows.MessageBox.Show("Are you sure you want to delete this folder?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (confirmation == MessageBoxResult.Yes)
            {
                FileSystem.DeleteDirectory(
                    path,
                    UIOption.OnlyErrorDialogs,
                    RecycleOption.SendToRecycleBin
                );

                //to move this logic in a different place
                SettingsHelper.RemoveNavigationPath(path);
                var pathSelectionContainer = _containersStore.AllContainers.FirstOrDefault(x => x.ContainerData.IsPathSelection);
                if (pathSelectionContainer != null)
                {
                    var item = pathSelectionContainer.ContainerData.Items.FirstOrDefault(x => x.FullPath == path);
                    if (item != null)
                    {
                        pathSelectionContainer.ContainerData.Items.Remove(item);
                        pathSelectionContainer.ItemListViewModel.Refresh();
                    }
                }
                return true;
            }

            return false;
        }

        private bool DeleteFile(string path)
        {
            var confirmation = System.Windows.MessageBox.Show("Are you sure you want to delete this file?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (confirmation == MessageBoxResult.Yes)
            {
                FileSystem.DeleteFile(
                    path,
                    UIOption.OnlyErrorDialogs,
                    RecycleOption.SendToRecycleBin
                );

                return true;
            }

            return false;
        }

        internal string CreateFolder(string elementPath)
        {
            if (string.IsNullOrEmpty(elementPath) || !Directory.Exists(elementPath))
                return "";

            int count = 0;
            var name = "New Folder";
            while (Directory.Exists(Path.Combine(elementPath, name)))
            {
                name = "New Folder " + (++count);
            }

            Directory.CreateDirectory(Path.Combine(elementPath, name));

            return Path.Combine(elementPath, name);
        }

        internal string? CreateTextDoc(string elementPath)
        {
            if (string.IsNullOrEmpty(elementPath) || !Directory.Exists(elementPath))
                return null;

            int count = 0;
            var name = "New Text Document.txt";
            while (File.Exists(Path.Combine(elementPath, name)))
            {
                name = "New Text Document " + (++count) + ".txt";
            }
            File.Create(Path.Combine(elementPath, name)).Dispose();
            return Path.Combine(elementPath, name);
        }

        internal ExplorerItem GetNode(string path)
        {
            var icon = FileIconHelper.GetSmallIcon(path);
            var imageSource = IconToImageSource(icon);
            imageSource.Freeze();
            return new ExplorerItem
            {
                Name = Path.GetFileName(path),
                FullPath = path,
                Icon = imageSource,
                IsDirectory = Directory.Exists(path)
            };
        }

        internal void RenameItem(string fullPath, string name)
        {
            var newPath = Path.Combine(Path.GetDirectoryName(fullPath), name);
            if (File.Exists(fullPath))
            {
                File.Move(fullPath, Path.Combine(Path.GetDirectoryName(fullPath), name));
            }
            else if (Directory.Exists(fullPath))
            {
                Directory.Move(fullPath, Path.Combine(Path.GetDirectoryName(fullPath), name));

                //update the saved paths
                var pathSelectionContainer = _containersStore.AllContainers.FirstOrDefault(x => x.ContainerData.IsPathSelection);
                if (pathSelectionContainer != null)
                {
                    var item = pathSelectionContainer.ContainerData.Items.FirstOrDefault(x => x.FullPath == fullPath);
                    if (item != null)
                    {
                        item.Name = name;
                        item.FullPath = Path.Combine(Path.GetDirectoryName(fullPath), name);
                        pathSelectionContainer.ItemListViewModel.Refresh();
                    }
                }
            }
        }
    }
}
