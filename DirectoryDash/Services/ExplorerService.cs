using DirectoryDash.Helpers;
using DirectoryDash.Models;
using System;
using System.Collections.Generic;
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
                if (!_clearViewCT.IsCancellationRequested)
                    Clear?.Invoke();
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
    }
}
